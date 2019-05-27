using System.Threading.Tasks;
using EdGps.Core;

namespace EdGps
{
    public class Gps
    {
        private JournalReader _reader;
        private StarSystem _system;

        public Gps(string directoryPath) {
            _reader = new JournalReader(directoryPath);
        }

        public async Task StartAsync() {
            _system = await StarSystem.LoadAsync();
            _reader.Start();
        }
    }
}