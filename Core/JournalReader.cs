using System.IO;
using System.Linq;

namespace Elite_Dangerous_Galactic_Positioning_System.Core
{
    public class JournalReader
    {
        public JournalReader(string journalDirectory) {
            var directory = new DirectoryInfo(journalDirectory);
            var journal = directory.GetFiles()
                .Where(f => f.Extension == ".log")
                .OrderByDescending(f => f.LastWriteTime)
                .First();
        }
    }
}