using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EdGps.Core.Models;

namespace EdGps.Core
{
    public class JournalReader
    {
        private DirectoryInfo _directory;
        private CancellationTokenSource _cancelReader = null;
        private FileSystemWatcher watcher = new FileSystemWatcher();
        private Task _task = null;
        private ReaderStatus _status = ReaderStatus.Idle;

        public event EventHandler<FsdJump> OnFsdJump;
        public event EventHandler<FssDiscoveryScan> OnFssDiscoveryScan;
        public event EventHandler<Body> OnBodyScan;
        public event EventHandler<DssScan> OnDssScan;
        public event EventHandler<bool> OnAllBodiesFound;
        public event EventHandler<StartJump> OnStartJump;
        public event EventHandler<bool> OnShutdown;

        public JournalReader(string journalDirectory) {
            _directory = new DirectoryInfo(journalDirectory);

            watcher.Path = journalDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess 
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName;
            watcher.Filter = "*.log";
            watcher.Created += OnCreatedAsync;
        }

        public void Start() {
            _cancelReader?.Dispose();
            _cancelReader = new CancellationTokenSource();            
            watcher.EnableRaisingEvents = true;
            _task = Task.Run(async () => await RunAsync(GetJournal()));
        }

        private async Task StopAsync() {
            _cancelReader.Cancel();
            while (_status != ReaderStatus.Stopped) await Task.Delay(1000);
        }

        private FileInfo GetJournal()
            => _directory.GetFiles()
                .Where(f => f.Extension == ".log")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

        private async Task RunAsync(FileInfo journalFile) {
            using (FileStream fs = journalFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
            using (StreamReader sr = new StreamReader(fs)) {
                _status = ReaderStatus.Active;
                while (!_cancelReader.IsCancellationRequested) {
                    while (!sr.EndOfStream) ReadEvent(Parser.ParseJson(sr.ReadLine()));
                    while (sr.EndOfStream && !_cancelReader.IsCancellationRequested) await Task.Delay(1000);
                }
            }
            _status = ReaderStatus.Stopped;
        }

        private async void OnCreatedAsync(object sender, FileSystemEventArgs e) {
            await StopAsync();
            Start();
        }

        public void ReadEvent(Dictionary<string, object> rawData) {
            if (!rawData.ContainsKey("event")) return;

            switch (rawData["event"]) {
                case "FSDJump":
                    OnFsdJump?.Invoke(this, Parser.ParseJournalEvent<FsdJump>(rawData));
                    break;
                case "FSSDiscoveryScan":
                    OnFssDiscoveryScan?.Invoke(this, Parser.ParseJournalEvent<FssDiscoveryScan>(rawData));
                    break;
                case "Scan":
                    var body = Parser.ParseScanBody(rawData);

                    if (rawData.ContainsKey("StarType")) {
                        body.Type = BodyType.Star;
                        body.SubType = rawData["StarType"].ToString();
                    } else if (rawData.ContainsKey("PlanetClass")) {
                        body.Type = BodyType.Planet;
                        body.SubType = rawData["PlanetClass"].ToString();
                        body.Terraformable = rawData["TerraformState"].ToString();
                    } else body.Type = BodyType.Belt;

                    OnBodyScan?.Invoke(this, body);
                    break;
                case "FSSAllBodiesFound":
                    OnAllBodiesFound?.Invoke(this, true);
                    break;
                case "SAAScanComplete":
                    OnDssScan?.Invoke(this, Parser.ParseJournalEvent<DssScan>(rawData));
                    break;
                case "StartJump":
                    OnStartJump?.Invoke(this, Parser.ParseJournalEvent<StartJump>(rawData));
                    break;
                case "Shutdown":
                    OnShutdown?.Invoke(this, true);
                    break;
                default:
                    return;
            }
        }
    }
}