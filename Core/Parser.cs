using System;
using System.Collections.Generic;
using EdGps.Core.Models;
using Newtonsoft.Json;

namespace EdGps.Core
{
    public static class Parser
    {
        private static Dictionary<string, BodyType> _starTypes = new Dictionary<string, BodyType>() {
            // Non-Sequence Stars
            { "H", BodyType.Black_Hole },
            { "SupermassiveBlackHole", BodyType.Black_Hole },
            { "N", BodyType.Neutron_Star },

            // Proto-Stars
            { "TTS", BodyType.Proto_Star },
            { "AeBe", BodyType.Proto_Star },
        };

        private static Dictionary<string, BodyType> _worldTypes = new Dictionary<string, BodyType>() {
            // Worlds
            { "Water world", BodyType.WaterWorld },
            { "Earthlike body", BodyType.EarthlikeWorld },
            { "Ammonia world", BodyType.AmmoniaWorld },
            { "High metal content body", BodyType.HighMetalContent },
            { "Metal rich body", BodyType.MetalRich }
        };

        public static BodyType ParseStarType(string starType) {
            if (starType.StartsWith("D")) return BodyType.White_Dwarf;
            else if (starType.StartsWith("W")) return BodyType.Wolf_Rayet;
            else if (starType.StartsWith("C")) return BodyType.Carbon_Star;
            else if (starType.Contains("SuperGiant")) return BodyType.Super_Giant;
            else if (starType.Contains("Giant")) return BodyType.Giant;

            if (_starTypes.ContainsKey(starType)) return _starTypes[starType];
            return BodyType.Star;
        }

        public static BodyType ParseWorldType(string worldType) {
            if (_worldTypes.ContainsKey(worldType)) return _worldTypes[worldType];
            if (worldType.Contains("gas giant", StringComparison.CurrentCultureIgnoreCase))
                if (worldType.Contains("class II gas giant")) return BodyType.GasGiant2;
                else return BodyType.GasGiant;
            return BodyType.Planet;
        }

        public static string SanitizeDirectory(string directory) 
            => directory.Replace('\'', ' ')
                        .Replace('&', ' ')
                        .Replace('"', ' ')
                        .TrimStart(' ')
                        .TrimEnd(' ');

        public static Dictionary<string, object> ParseJson(string line) 
            => JsonConvert.DeserializeObject<Dictionary<string, object>>(line);

        public static T ParseJournalEvent<T>(Dictionary<string, object> rawData) where T : class, IJournalEvent
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(rawData));
        
        public static Body ParseScanBody(Dictionary<string, object> rawData) {
            var isDiscovered = rawData.ContainsKey("WasDiscovered");
            var isMapped = rawData.ContainsKey("WasMapped");
            var body = new Body(JsonConvert.DeserializeObject<ScanBody>(JsonConvert.SerializeObject(rawData)));

            if (!isDiscovered) body.Discovered = true;
            if (!isMapped) body.Mapped = true;

            return body;
        }
    }
}