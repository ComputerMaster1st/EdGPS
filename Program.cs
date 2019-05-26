using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EDCurrentSystem.Models;

namespace EDCurrentSystem
{
    class Program
    {
        private ConsoleOutput conOutput = new ConsoleOutput();

        static void Main(string[] args) {
            Console.Write("E:D Journal Log Directory :");
            var rawDirectory = Console.ReadLine()
                .Replace('\'', ' ')
                .Replace('&', ' ')
                .TrimStart(' ')
                .TrimEnd(' ');

            if (rawDirectory.Length < 1) {
                Console.WriteLine("Please specify the directory where E:D Journal logs are kept.");
                return;
            }

            var directory = new DirectoryInfo(rawDirectory);
            var log = directory.GetFiles().Where(f => f.Extension == ".log")
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            
            if (!Directory.Exists(Directories.SystemDir)) Directory.CreateDirectory(Directories.SystemDir);

            new Program().RunAsync(log.ToString()).GetAwaiter().GetResult();
        }

        public async Task RunAsync(string filename) {
            conOutput.SendToConsole();
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (StreamReader sr = new StreamReader(fs)) {
                    while (true) {
                        while (!sr.EndOfStream) ReadEvent(Parser.ParseJson(sr.ReadLine()));
                        while (sr.EndOfStream) await Task.Delay(1000);
                        ReadEvent(Parser.ParseJson(sr.ReadLine()));          
                    }
                }
            }
        }

        public void ReadEvent(Dictionary<string, object> rawData) {
            if (!rawData.ContainsKey("event")) return;

            switch (rawData["event"]) {
                case "FSDJump":
                    var jump = Parser.ParseFsdJump(rawData);
                    conOutput.SetNewSystem(jump.SystemName, jump.SystemPosition);
                    break;
                case "FSSDiscoveryScan":
                    var discovery = Parser.ParseDiscovery(rawData);
                    conOutput.CurrentSystem.TotalBodies = discovery.BodyCount;
                    conOutput.CurrentSystem.TotalNonBodies = discovery.NonBodyCount;
                    conOutput.CurrentSystem.IsHonked = true;
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

                    conOutput.CurrentSystem.AddBody(body);
                    break;
                case "FSSAllBodiesFound":
                    conOutput.CurrentSystem.IsComplete = true;
                    break;
                case "SAAScanComplete":
                    var scan = Parser.ParseDssScan(rawData);
                    conOutput.CurrentSystem.DssScanned(scan);
                    break;
                case "StartJump":
                    var target = Parser.ParseStartJump(rawData);
                    conOutput.SetNextSystem(target.SystemName);
                    break;
                case "Shutdown":
                    conOutput.CurrentSystem.Save();
                    break;
                default:
                    return;
            }

            conOutput.SendToConsole();
        }
    }
}
