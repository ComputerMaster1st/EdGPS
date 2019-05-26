using System.IO;

namespace EDCurrentSystem
{
    public class JournalReader
    {
        private string journalPath;
        private FileSystemWatcher watcher = new FileSystemWatcher();

        public JournalReader(string journalDirectory) {
            watcher.Path = journalDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess 
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName;
            watcher.Filter = "*.log";
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs args) {

        }
    }
}