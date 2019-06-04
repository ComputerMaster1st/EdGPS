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
        private StarSystem _system = null;
        private string _nextSystem = null;
        private bool _isReady = false;

        public Gps(string directoryPath) {
            _reader = new JournalReader(directoryPath);
            _writer = new ConsoleWriter();
        }

        public void Start() {
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
            _reader.Start();
            PlaySound(VoiceType.Initializing);
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
            _system.AddBody(body);
            _writer.Write(_system, _nextSystem);

            if (!string.IsNullOrEmpty(body.Terraformable)) PlaySound(VoiceType.Terraformable);
            else if (body.SubType == "Water world") PlaySound(VoiceType.Water);
            else if (body.SubType == "Earthlike body") PlaySound(VoiceType.Earth);
            else if (body.SubType == "Ammonia world") PlaySound(VoiceType.Ammonia);
        }

        private void OnAllBodiesFound(object sender, bool isAllFound) {
            _system.IsComplete = isAllFound;
            _writer.Write(_system, _nextSystem);
            PlaySound(VoiceType.Identified);
        }

        private void PlaySound(VoiceType response) {
            if (!_isReady) return;
            VoicePlayer.Play(response);
        }
    }
}