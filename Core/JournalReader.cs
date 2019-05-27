using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elite_Dangerous_Galactic_Positioning_System.Core
{
    public class JournalReader
    {
        private CancellationTokenSource _cancelReader = null;
        private FileSystemWatcher watcher = new FileSystemWatcher();

        public JournalReader(string journalDirectory) {
            var directory = new DirectoryInfo(journalDirectory);
            var journalPath = directory.GetFiles()
                .Where(f => f.Extension == ".log")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            watcher.Path = journalDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess 
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName;
            watcher.Filter = "*.log";
            watcher.Created += OnCreated;
        }

        public async Task StartAsync() {
            _cancelReader?.Dispose();
            _cancelReader = new CancellationTokenSource();
        }

        private async Task RunAsync(FileInfo journalFile, CancellationToken token) {
            throw new NotImplementedException();
        }

        private void OnCreated(object sender, FileSystemEventArgs e) {
            throw new NotImplementedException();
        }
    }
}