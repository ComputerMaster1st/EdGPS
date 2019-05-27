using Newtonsoft.Json;

namespace EdGps.Core.Models
{
    public class StartJump : IJournalEvent
    {
        public string JumpType { get; set; }
        public string SystemName { get; set; }

        public JournalEventType JournalEvent => JournalEventType.StartJump;

        [JsonConstructor]
        public StartJump(string jumpType, string starSystem) {
            JumpType = jumpType;
            SystemName = starSystem;
        }
    }
}