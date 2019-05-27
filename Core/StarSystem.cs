using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EdGps.Core.Models;
using Newtonsoft.Json;

namespace EdGps.Core
{
    [JsonObject(MemberSerialization.Fields)]
    public class StarSystem
    {
        public string Name { get; }

        public double Latitude { get; }
        public double Longitude { get; }
        public double Elevation { get; }

        public int TotalBodies { get; set; } = 0;
        public int TotalNonBodies { get; set; } = 0;
        public bool IsComplete { get; set; } = false;
        public bool IsHonked { get; set; } = false;

        public List<Body> Bodies { get; private set; } = new List<Body>();

        [JsonConstructor]
        private StarSystem(string name, double latitude, double longitude, double elevation) {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }
        
        public StarSystem(string name, List<double> coordinates) {
            Name = name;
            Latitude = coordinates[2];
            Longitude = coordinates[0];
            Elevation = coordinates[1];
        }

        public void AddBody(Body body) {
            body.Parents.Reverse();
            AddSubBody(Bodies, body);
        }

        public void DssScanned(DssScan scan) => MarkDssScanned(Bodies, scan);

        private void MarkDssScanned(List<Body> bodies, DssScan scan) {
            var body = bodies.FirstOrDefault(f => f.Id == scan.BodyId);
            
            if (body is null)
                foreach (var subBody in bodies)
                    MarkDssScanned(subBody.SubBodies, scan);
            else body.IsDssScanned = true;
        }

        private void AddSubBody(List<Body> bodies, Body newBody, int step = 0) {
            Body parentBody;

            if (newBody.Parents.Count < (step+1)) {
                parentBody = bodies.FirstOrDefault(f => f.Id == newBody.Id);

                if (parentBody is null) {
                    bodies.Add(newBody);
                    return;
                }
                if (string.IsNullOrEmpty(parentBody.Name)) {
                    parentBody.Discovered = newBody.Discovered;
                    parentBody.Distance = newBody.Distance;
                    parentBody.Mapped = newBody.Mapped;
                    parentBody.Name = newBody.Name;
                    parentBody.SubType = newBody.SubType;
                    parentBody.Type = newBody.Type;

                    return;
                } else return;
            }

            parentBody = bodies.FirstOrDefault(f => f.Id == newBody.Parents[step]);
            if (parentBody is null) {
                parentBody = new Body(newBody.Parents[step]);
                bodies.Add(parentBody);
            }
            
            AddSubBody(parentBody.SubBodies, newBody, step+1);
        }

        public async static Task<StarSystem> LoadAsync(string systemName = null) {
            if (!string.IsNullOrWhiteSpace(systemName)) {
                if (File.Exists($"{Directories.SystemDir}/{systemName}.json"))
                    return JsonConvert.DeserializeObject<StarSystem>(await File.ReadAllTextAsync($"{Directories.SystemDir}/{systemName}.json"));
                else return null;
            }

            var directory = new DirectoryInfo(Directories.SystemDir);
            var files = directory.GetFiles();

            if (files.Count() < 1) return null;

            var log = files.Where(f => f.Extension == ".json")
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            
            return JsonConvert.DeserializeObject<StarSystem>(await File.ReadAllTextAsync(log.ToString()));
        }

        public async Task SaveAsync() {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            await File.WriteAllTextAsync($"{Directories.SystemDir}/{Name}.json", json);
        }
    }
}