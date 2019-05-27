namespace EdGps.Core.Models
{
    public class FssDiscoveryScan : IJournalEvent
    {
        public int BodyCount { get; set; }
        public int NonBodyCount { get; set; }

        public JournalEventType JournalEvent => JournalEventType.FssDiscoveryScan;

        public FssDiscoveryScan(int bodyCount, int nonBodyCount) {
            BodyCount = bodyCount;
            NonBodyCount = nonBodyCount;
        }
    }
}