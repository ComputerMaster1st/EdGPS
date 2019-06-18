using Newtonsoft.Json;

namespace EdGps.Core.Models
{
    public class DssScan : IJournalEvent
    {
        public int BodyId { get; }
        public int ProbesUsed { get; }
        public int EfficiencyTarget { get; }

        public JournalEventType JournalEvent => JournalEventType.DssScanComplete;

        [JsonConstructor]
        public DssScan(int bodyId, int probesUsed, int efficiencyTarget) {
            BodyId = bodyId;
            ProbesUsed = probesUsed;
            EfficiencyTarget = efficiencyTarget;
        }
    }
}