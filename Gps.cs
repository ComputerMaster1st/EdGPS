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

        public Gps(string directoryPath) {
            _reader = new JournalReader(directoryPath);
            _writer = new ConsoleWriter();
        }

        public async Task StartAsync() {
            _reader.OnAllBodiesFound += OnAllBodiesFoundAsync;
            _reader.OnBodyScan += OnBodyScanAsync;
            _reader.OnDssScan += OnSurfaceScanAsync;
            _reader.OnFsdJump += OnEnteringNewSystemAsync;
            _reader.OnFssDiscoveryScan += OnSystemHonkAsync;
            _reader.OnShutdown += OnShutdownAsync;
            _reader.OnStartJump += OnEnteringHyperspaceAsync;

            _system = await StarSystem.LoadAsync() ?? new StarSystem("Waiting...", new List<double>() { 0, 0, 0 });
            _reader.Start();
        }

        private async void OnEnteringHyperspaceAsync(object sender, StartJump target) {
            _nextSystem = target.SystemName;
            await WriteAndSaveAsync();
        }

        private async void OnShutdownAsync(object sender, bool e) => await WriteAndSaveAsync();

        private async void OnSystemHonkAsync(object sender, FssDiscoveryScan scan) {
            _system.TotalBodies = scan.BodyCount;
            _system.TotalNonBodies = scan.NonBodyCount;
            _system.IsHonked = true;
            await WriteAndSaveAsync();
        }

        private async void OnEnteringNewSystemAsync(object sender, FsdJump system) {
            _system = new StarSystem(system.Name, system.Coordinates);
            await WriteAndSaveAsync();
        }

        private async void OnSurfaceScanAsync(object sender, DssScan scan) {
            _system.DssScanned(scan);
            await WriteAndSaveAsync();
        }

        private async void OnBodyScanAsync(object sender, Body body) {
            _system.AddBody(body);
            await WriteAndSaveAsync();
        }

        private async void OnAllBodiesFoundAsync(object sender, bool isAllFound) {
            _system.IsComplete = isAllFound;
            await WriteAndSaveAsync();
        }

        private async Task WriteAndSaveAsync() {
            await _system.SaveAsync();
            _writer.Write(_system, _nextSystem);
        }
    }
}