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
            { "N", BodyType.Neutron_Star },

            // White Dwarfs
            { "DA", BodyType.White_Dwarf },
            { "DAB", BodyType.White_Dwarf },
            { "DAV", BodyType.White_Dwarf },
            { "DAZ", BodyType.White_Dwarf },
            { "DC", BodyType.White_Dwarf },

            // Wolf-Rayet
            { "WO", BodyType.Wolf_Rayet },
            { "WN", BodyType.Wolf_Rayet },

            // Proto-Stars
            { "TTS", BodyType.T_Tauri_Star },
            { "AeBe", BodyType.Herbig_AeBe },

            // Giants
            { "M_RedGiant", BodyType.Red_Giant }
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