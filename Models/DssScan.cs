namespace EDCurrentSystem.Models
{
    public class DssScan
    {
        public int BodyId { get; }

        public DssScan(int bodyId) => BodyId = bodyId;
    }
}