using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EDCurrentSystem.Models
{
    public class Body
    {
        public int Id { get; }
        public string Name { get; set; } = string.Empty;
        public BodyType Type { get; set; } = BodyType.Other;
        public string SubType { get; set; } = string.Empty;
        public List<int> Parents { get; set; } = new List<int>();
        public double Distance { get; set; } = -1;
        public bool Discovered { get; set; }
        public bool Mapped { get; set; }
        public string Terraformable { get; set; } = string.Empty;
        public bool IsDssScanned { get; set; }

        public List<Body> SubBodies { get; set; } = new List<Body>();

        [JsonConstructor]
        private Body(int id, string name, List<int> parents, double distance, string subType, bool discovered, bool mapped) {
            Id = id;
            Name = name;
            SubType = subType;
            Parents = parents;
            Distance = distance;
            Discovered = discovered;
            Mapped = mapped;
        }

        public Body(ScanBody body) {
            Id = body.Id;
            Name = body.Name;
            Parents = body.Parents;
            Distance = body.Distance;
            SubType = body.SubType;
            Discovered = body.Discovered;
            Mapped = body.Mapped;
            Terraformable = body.Terraformable;
        }
        
        public Body(int bodyId) => Id = bodyId;

        private List<int> ParseParents(List<Parent> parents) {
            var newParents = new List<int>();

            if (parents is null) return newParents;

            foreach (var parent in parents) {
                if (parent.Null > -1) {
                    newParents.Add(parent.Null);
                    continue;
                }
                if (parent.Planet > -1) {
                    newParents.Add(parent.Planet);
                    continue;
                }
                if (parent.Ring > -1) {
                    newParents.Add(parent.Ring);
                    continue;
                }
                if (parent.Star > -1) {
                    newParents.Add(parent.Star);
                    continue;
                }
            }
            
            return newParents;
        }
    }
}