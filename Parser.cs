using System.Collections.Generic;
using EDCurrentSystem.Models;
using Newtonsoft.Json;

namespace EDCurrentSystem
{
    public static class Parser
    {
        public static Dictionary<string, object> ParseJson(string line) 
            => JsonConvert.DeserializeObject<Dictionary<string, object>>(line);

        public static StartJump ParseStartJump(Dictionary<string, object> rawData) 
            => JsonConvert.DeserializeObject<StartJump>(JsonConvert.SerializeObject(rawData));

        public static FssDiscoveryScan ParseDiscovery(Dictionary<string, object> rawData) 
            => JsonConvert.DeserializeObject<FssDiscoveryScan>(JsonConvert.SerializeObject(rawData));

        public static FsdJump ParseFsdJump(Dictionary<string, object> rawData) 
            => JsonConvert.DeserializeObject<FsdJump>(JsonConvert.SerializeObject(rawData));

        public static Body ParseScanBody(Dictionary<string, object> rawData) {
            var isDiscovered = rawData.ContainsKey("WasDiscovered");
            var isMapped = rawData.ContainsKey("WasMapped");
            var body = new Body(JsonConvert.DeserializeObject<ScanBody>(JsonConvert.SerializeObject(rawData)));

            if (!isDiscovered) body.Discovered = true;
            if (!isMapped) body.Mapped = true;

            return body;
        }
        
        public static DssScan ParseDssScan(Dictionary<string, object> rawData) 
            => JsonConvert.DeserializeObject<DssScan>(JsonConvert.SerializeObject(rawData));
    }
}