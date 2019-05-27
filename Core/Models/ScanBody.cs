using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EdGps.Core.Models
{
    public class ScanBody
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

        [JsonConstructor]
        private ScanBody(int bodyId, string bodyName, List<Parent> parents, double distanceFromArrivalLS, string type, bool wasDiscovered, bool wasMapped) {
            Id = bodyId;
            Name = bodyName;
            Parents = ParseParents(parents);
            Distance = Math.Round(distanceFromArrivalLS, 0);
            SubType = type;
            Discovered = wasDiscovered;
            Mapped = wasMapped;
        }

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