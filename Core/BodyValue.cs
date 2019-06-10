using System;

namespace EdGps.Core
{
    public static class BodyValue
    {
        private const double StarFactor = 66.25;

        private const int Star = 1200;
        private const int NeutronStar = 22628;
        private const int BlackHole = 22628;
        private const int WhiteDwarf = 14057;

        public static int GetStarValue(BodyType type, double mass) {
            switch (type) {
                case BodyType.Star:
                    return (int)Math.Round(Star + ((mass * Star) / StarFactor));
                case BodyType.Black_Hole:
                    return (int)Math.Round(BlackHole + ((mass * BlackHole) / StarFactor));
                case BodyType.Neutron_Star:
                    return (int)Math.Round(NeutronStar + ((mass * NeutronStar) / StarFactor));
                case BodyType.White_Dwarf:
                    return (int)Math.Round(WhiteDwarf + ((mass * WhiteDwarf) / StarFactor));
                default:
                    return 0;
            }
        }
    }
}