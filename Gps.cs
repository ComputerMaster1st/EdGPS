using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdGps.Core;
using EdGps.Core.Models;

namespace EdGps
{
    public class Gps
    {
        private JournalReader _reader;
        private ConsoleWriter _writer;
        private VoicePlayer _voice = new VoicePlayer();
        private StarSystem _system = null;
        private string _nextSystem = null;
        private bool _isReady = false;
        private bool _voiceEnabled;

        public Gps(Config config) {
            _reader = new JournalReader(config.JournalPath);
            _writer = new ConsoleWriter();
            _voiceEnabled = config.VoiceEnabled;
        }

        public void Start(bool build = false) {
            _reader.OnAllBodiesFound += OnAllBodiesFound;
            _reader.OnBodyScan += OnBodyScan;
            _reader.OnDssScan += OnSurfaceScan;
            _reader.OnFsdJump += OnEnteringNewSystem;
            _reader.OnFssDiscoveryScan += OnSystemHonk;
            _reader.OnReady += OnReady;
            _reader.OnShutdown += OnShutdown;
            _reader.OnStartJump += OnEnteringHyperspace;

            _system = StarSystem.Load() ?? new StarSystem("Waiting...", new List<double>() { 0, 0, 0 });
            Console.Title = $"Elite: Dangerous | Global Positioning System | {_system.Name}";
            _writer.Write(_system);

            if (build) _reader.Build();
            _reader.Start();
        }

        private void OnEnteringHyperspace(object sender, StartJump target) {
            _system.Save();
            _nextSystem = target.SystemName;
            _writer.Write(_system, _nextSystem);
            PlaySound(VoiceType.Jumping);
        }

        private void OnShutdown(object sender, bool e) => _system.Save();
        
        private void OnReady(object sender, bool e) {
            _isReady = true;
            PlaySound(VoiceType.Standby);
        }

        private void OnSystemHonk(object sender, FssDiscoveryScan scan) {
            _system.TotalBodies = scan.BodyCount;
            _system.TotalNonBodies = scan.NonBodyCount;
            _system.IsHonked = true;
            _writer.Write(_system, _nextSystem);
            if (!_system.IsComplete) PlaySound(VoiceType.Unidentified);
        }

        private void OnEnteringNewSystem(object sender, FsdJump system) {
            _system = StarSystem.Load(system.Name) ?? new StarSystem(system.Name, system.Coordinates);
            Console.Title = $"Elite: Dangerous | Global Positioning System | {_system.Name}";
            _nextSystem = null;
            _writer.Write(_system, _nextSystem);
            PlaySound(VoiceType.Dropping);
        }

        private void OnSurfaceScan(object sender, DssScan scan) {
            _system.DssScanned(scan);
            _writer.Write(_system, _nextSystem);
            PlaySound(VoiceType.Dss);
        }

        private void OnBodyScan(object sender, Body body) {
            switch (body.SubType) {
                // Star Class
                case "H":
                    body.Type = BodyType.Black_Hole;
                    PlaySound(VoiceType.BlackHole);
                    break;
                case "N":
                    body.Type = BodyType.Neutron_Star;
                    PlaySound(VoiceType.NeutronStar);
                    break;
                case "DAZ":
                    body.Type = BodyType.White_Dwarf;
                    PlaySound(VoiceType.WhiteDwarf);
                    break;
                case "TTS":
                    body.Type = BodyType.T_Tauri_Star;
                    break;
                // World Class
                case string val when val.Contains("gas giant", StringComparison.CurrentCultureIgnoreCase):
                    if (val.Contains("class II gas giant")) body.Type = BodyType.GasGiant2;
                    else body.Type = BodyType.GasGiant;
                    break;
                case "Water world":
                    PlaySound(VoiceType.Water);
                    body.Type = BodyType.WaterWorld;
                    break;
                case "Earthlike body":
                    PlaySound(VoiceType.Earth);
                    body.Type = BodyType.EarthlikeWorld;
                    break;
                case "Ammonia world":
                    PlaySound(VoiceType.Ammonia);
                    body.Type = BodyType.AmmoniaWorld;
                    break;
                case "High metal content body":
                    body.Type = BodyType.HighMetalContent;
                    break;
                case "Metal rich body":
                    body.Type = BodyType.MetalRich;
                    break;
                default:
                    break;
            }

            _system.AddBody(body);
            _writer.Write(_system, _nextSystem);
            if (_system.IsComplete) return;

            if (!string.IsNullOrEmpty(body.Terraformable)) PlaySound(VoiceType.Terraformable);
        }

        private void OnAllBodiesFound(object sender, bool isAllFound) {
            if (!_system.IsComplete) PlaySound(VoiceType.Identified);
            _system.IsComplete = isAllFound;
            _writer.Write(_system, _nextSystem);
        }

        private void PlaySound(VoiceType response) {
            if (!_voiceEnabled) return;
            if (!_isReady) return;
            _ = Task.Run(async () => await _voice.Play(response));
        }
    }
}