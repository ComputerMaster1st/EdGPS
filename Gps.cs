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
        private int _soundVolume;

        public Gps(Config config) {
            _reader = new JournalReader(config.JournalPath);
            _writer = new ConsoleWriter();
            _voiceEnabled = config.VoiceEnabled;
            _soundVolume = config.SoundVolume;
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
            Console.Title = $"Elite: Dangerous | Galactic Positioning System | {_system.Name}";
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
            Console.Title = $"Elite: Dangerous | Galactic Positioning System | {_system.Name}";
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
            if (!body.Name.Contains(_system.Name)) return;
            _system.AddBody(body);
            _writer.Write(_system, _nextSystem);
            if (_system.IsComplete) return;

            if (!string.IsNullOrEmpty(body.Terraformable)) PlaySound(VoiceType.Terraformable);

            switch (body.Type) {
                // Star Class
                case BodyType.Black_Hole:
                    PlaySound(VoiceType.BlackHole);
                    break;
                case BodyType.Neutron_Star:
                    PlaySound(VoiceType.NeutronStar);
                    break;
                case BodyType.White_Dwarf:
                    PlaySound(VoiceType.WhiteDwarf);
                    break;

                // World Class
                case BodyType.WaterWorld:
                    PlaySound(VoiceType.Water);
                    break;
                case BodyType.EarthlikeWorld:
                    PlaySound(VoiceType.Earth);
                    break;
                case BodyType.AmmoniaWorld:
                    PlaySound(VoiceType.Ammonia);
                    break;
                default:
                    break;
            }
        }

        private void OnAllBodiesFound(object sender, bool isAllFound) {
            if (!_system.IsComplete) PlaySound(VoiceType.Identified);
            _system.IsComplete = isAllFound;
            _writer.Write(_system, _nextSystem);
        }

        private void PlaySound(VoiceType response) {
            if (!_voiceEnabled) return;
            if (!_isReady) return;
            _ = Task.Run(async () => await _voice.Play(response, _soundVolume));
        }
    }
}