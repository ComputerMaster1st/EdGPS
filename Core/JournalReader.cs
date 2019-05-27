using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elite_Dangerous_Galactic_Positioning_System.Core
{
    public class JournalReader
    {
        private DirectoryInfo _directory;
        private CancellationTokenSource _cancelReader = null;
        private FileSystemWatcher watcher = new FileSystemWatcher();
        private Task _task = null;

        public JournalReader(string journalDirectory) {
            _directory = new DirectoryInfo(journalDirectory);

            watcher.Path = journalDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess 
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName;
            watcher.Filter = "*.log";
            watcher.Created += OnCreatedAsync;
            watcher.EnableRaisingEvents = true;
        }

        public async Task StartAsync() {
            _cancelReader?.Dispose();
            _cancelReader = new CancellationTokenSource();            
            _task = Task.Run(async () => await RunAsync(GetJournal(), _cancelReader.Token));
            _task.Start();
        }

        private async Task StopAsync() {
            _cancelReader.Cancel();
            await _task;
        }

        private FileInfo GetJournal()
            => _directory.GetFiles()
                .Where(f => f.Extension == ".log")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

        private async Task RunAsync(FileInfo journalFile, CancellationToken token) {
            using (FileStream fs = journalFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
            using (StreamReader sr = new StreamReader(fs)) {
                while (!token.IsCancellationRequested) {
                    while (!sr.EndOfStream) {
                        // TODO: Parser Json
                    }
                    while (sr.EndOfStream) await Task.Delay(1000);

                    // TODO: Parser Json
                }
            }
        }

        private async void OnCreatedAsync(object sender, FileSystemEventArgs e) {
            await StopAsync();
            await StartAsync();
        }

        public void ReadEvent(Dictionary<string, object> rawData) {
            
        }
    }
}