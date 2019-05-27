using System.Collections.Generic;
using EdGps.Core.Models;
using Newtonsoft.Json;

namespace EdGps.Core
{
    public static class Parser
    {
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