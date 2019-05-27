using System.Threading.Tasks;
using EdGps.Core;

namespace EdGps
{
    public class Gps
    {
        private JournalReader _reader;

        public Gps(string directoryPath) {
            _reader = new JournalReader(directoryPath);
        }

        public void Start() {
            _reader.Start();
        }
    }
}