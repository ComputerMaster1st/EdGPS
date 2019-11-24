using System.Collections.Generic;
using Newtonsoft.Json;

namespace EdGps.Core.Models
{
    public class Body
    {
        public int Id { get; }
        public string Name { get; set; } = string.Empty;
        public BodyType Type { get; set; } = BodyType.Other;
        public string SubType { get; set; } = string.Empty;
        public double Mass { get; set; } = 0;
        public List<int> Parents { get; set; } = new List<int>();
        public string StarSystem { get; set; } = string.Empty;
        public double Distance { get; set; } = -1;
        public bool Discovered { get; set; }
        public bool Mapped { get; set; }
        public bool FirstMapped { get; set; }
        public string Terraformable { get; set; } = string.Empty;
        public bool IsDssScanned { get; set; }
        public bool DssEfficiencyAchieved { get; set; }

        public List<Body> SubBodies { get; set; } = new List<Body>();

        [JsonConstructor]
        private Body(int id, string name, List<int> parents, string starSystem, double distance, string subType, bool discovered, bool mapped) {
            Id = id;
            Name = name;
            SubType = subType;
            Parents = parents;
            StarSystem = starSystem;
            Distance = distance;
            Discovered = discovered;
            Mapped = mapped;
        }

        public Body(ScanBody body) {
            Id = body.Id;
            Name = body.Name;
            Parents = body.Parents;
            StarSystem = body.StarSystem;
            Distance = body.Distance;
            SubType = body.SubType;
            Discovered = body.Discovered;
            Mapped = body.Mapped;
            Terraformable = body.Terraformable;
        }
        
        public Body(int bodyId) => Id = bodyId;
    }
}