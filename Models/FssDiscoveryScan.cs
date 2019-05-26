namespace EDCurrentSystem.Models
{
    public class FssDiscoveryScan
    {
        public int BodyCount { get; set; }
        public int NonBodyCount { get; set; }

        public FssDiscoveryScan(int bodyCount, int nonBodyCount) {
            BodyCount = bodyCount;
            NonBodyCount = nonBodyCount;
        }
    }
}