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
        private string _nextSystem = string.Empty;

        public Gps(string directoryPath) {
            _reader = new JournalReader(directoryPath);
            _writer = new ConsoleWriter();
        }

        public async Task StartAsync() {
            _reader.OnAllBodiesFound += OnAllBodiesFoundAsync;
            _reader.OnBodyScan += OnBodyScanAsync;
            _reader.OnFsdJump += OnEnteringNewSystemAsync;

            _system = await StarSystem.LoadAsync() ?? new StarSystem("Waiting...", new List<double>() { 0, 0, 0 });
            _reader.Start();
        }

        private async void OnBodyScanAsync(object sender, Body body) {
            _system.AddBody(body);
            _writer.Write(_system);
            await _system.SaveAsync();
        }

        private async void OnAllBodiesFoundAsync(object sender, bool isAllFound) {
            _system.IsComplete = isAllFound;
            _writer.Write(_system);
            await _system.SaveAsync();
        }

        private async void OnEnteringNewSystemAsync(object sender, FsdJump system) {
            _system = new StarSystem(system.Name, system.Coordinates);
            _writer.Write(_system);
            await _system.SaveAsync();
        }
    }
}