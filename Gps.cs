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
            _reader.OnFsdJump += OnEnteringNewSystem;

            _system = await StarSystem.LoadAsync() ?? new StarSystem("Waiting...", new List<double>() { 0, 0, 0 });
            _reader.Start();
        }

        private void OnEnteringNewSystem(object sender, FsdJump system) {
            _system = new StarSystem(system.Name, system.Coordinates);
            _writer.Write(_system);
        }
    }
}