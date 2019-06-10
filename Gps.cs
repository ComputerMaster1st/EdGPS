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

        private async void OnEnteringHyperspace(object sender, StartJump target) {
            _system.Save();
            _nextSystem = target.SystemName;
            _writer.Write(_system, _nextSystem);
            await PlaySound(VoiceType.Jumping);
        }

        private void OnShutdown(object sender, bool e) => _system.Save();
        
        private async void OnReady(object sender, bool e) {
            _isReady = true;
            await PlaySound(VoiceType.Standby);
        }

        private async void OnSystemHonk(object sender, FssDiscoveryScan scan) {
            _system.TotalBodies = scan.BodyCount;
            _system.TotalNonBodies = scan.NonBodyCount;
            _system.IsHonked = true;
            _writer.Write(_system, _nextSystem);
            if (!_system.IsComplete) await PlaySound(VoiceType.Unidentified);
        }

        private async void OnEnteringNewSystem(object sender, FsdJump system) {
            _system = StarSystem.Load(system.Name) ?? new StarSystem(system.Name, system.Coordinates);
            Console.Title = $"Elite: Dangerous | Global Positioning System | {_system.Name}";
            _nextSystem = null;
            _writer.Write(_system, _nextSystem);
            await PlaySound(VoiceType.Dropping);
        }

        private async void OnSurfaceScan(object sender, DssScan scan) {
            _system.DssScanned(scan);
            _writer.Write(_system, _nextSystem);
            await PlaySound(VoiceType.Dss);
        }

        private async void OnBodyScan(object sender, Body body) {
            switch (body.SubType) {
                // Star Class
                case "H":
                    body.Type = BodyType.Black_Hole;
                    await PlaySound(VoiceType.BlackHole);
                    break;
                case "N":
                    body.Type = BodyType.Neutron_Star;
                    await PlaySound(VoiceType.NeutronStar);
                    break;
                case "DAZ":
                    body.Type = BodyType.White_Dwarf;
                    await PlaySound(VoiceType.WhiteDwarf);
                    break;
                case "TTS":
                    body.Type = BodyType.T_Tauri_Star;
                    break;
                default:
                    break;
            }

            _system.AddBody(body);
            _writer.Write(_system, _nextSystem);
            if (_system.IsComplete) return;

            if (!string.IsNullOrEmpty(body.Terraformable)) await PlaySound(VoiceType.Terraformable);

            switch (body.SubType) {
                case "Water world":
                    await PlaySound(VoiceType.Water);
                    break;
                case "Earthlike body":
                    await PlaySound(VoiceType.Earth);
                    break;
                case "Ammonia world":
                    await PlaySound(VoiceType.Ammonia);
                    break;
            }
        }

        private async void OnAllBodiesFound(object sender, bool isAllFound) {
            _system.IsComplete = isAllFound;
            _writer.Write(_system, _nextSystem);
            await PlaySound(VoiceType.Identified);
        }

        private async Task PlaySound(VoiceType response) {
            if (!_voiceEnabled) return;
            if (!_isReady) return;
            await _voice.Play(response);
        }
    }
}