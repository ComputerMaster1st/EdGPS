using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EdGps.Core
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

        public void Start() {
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
                    while (!sr.EndOfStream) ReadEvent(Parser.ParseJson(sr.ReadLine()));
                    while (sr.EndOfStream) await Task.Delay(1000);
                    ReadEvent(Parser.ParseJson(sr.ReadLine()));
                }
            }
        }

        private async void OnCreatedAsync(object sender, FileSystemEventArgs e) {
            await StopAsync();
            Start();
        }

        public void ReadEvent(Dictionary<string, object> rawData) {
            if (!rawData.ContainsKey("event")) return;

            switch (rawData["event"]) {
                case "FSDJump":
                    // TODO: Create FSDJump Event
                    break;
                case "FSSDiscoveryScan":
                    // TODO: Create FSSDiscoveryScan Event
                    break;
                case "Scan":
                    // TODO: Create Scan Event
                    break;
                case "FSSAllBodiesFound":
                    // TODO: Create FSSAllBodiesFound Event
                    break;
                case "SAAScanComplete":
                    // TODO: Create SAAScanComplete Event
                    break;
                case "StartJump":
                    // TODO: Create StartJump Event
                    break;
                case "Shutdown":
                    // TODO: Create Shutdown Event
                    break;
                default:
                    return;
            }
        }
    }
}