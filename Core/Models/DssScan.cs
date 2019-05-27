namespace EdGps.Core.Models
{
    public class DssScan : IJournalEvent
    {
        public int BodyId { get; }

        public JournalEventType JournalEvent => JournalEventType.DssScanComplete;

        public DssScan(int bodyId) => BodyId = bodyId;
    }
}