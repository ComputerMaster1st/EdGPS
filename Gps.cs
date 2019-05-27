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
            _system = StarSystem.Load();
        }

        public void Start() {
            _reader.Start();
        }
    }
}