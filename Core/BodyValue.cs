using System;

namespace EdGps.Core
{
    public static class BodyValue
    {
        // Star Base Values
        private const double StarFactor = 66.25;

        private const int Star = 1200;
        private const int NeutronStar = 22628;
        private const int BlackHole = 22628;
        private const int WhiteDwarf = 14057;

        // World Base Values
        private const double WorldFactor = 0.56591828;

        private const int MetalRich = 21790;
        private const int AmmoniaWorld = 96932;
        private const int GasGiant = 1656;
        private const int GasGiantClass2 = 9654;
        private const int HighMetalContent = 9654;
        private const int WaterWorld = 64831;
        private const int EarthlikeWorld = 181126;
        private const int Other = 300;

        // World Terraformable Values
        private const int TerraformableHighMetalContent = 100677 + HighMetalContent;
        private const int TerraformableWaterWorld = 116295 + WaterWorld;
        private const int TerraformableOther = 93328 + Other;

        public static int GetBodyValue(BodyType type, double mass) {
            switch (type) {
                // Stars
                case BodyType.Star:
                    var calc = Star + ((mass * Star) / StarFactor);
                    return (int)Math.Round(calc);
                case BodyType.Black_Hole:
                    return (int)Math.Round(BlackHole + ((mass * BlackHole) / StarFactor));
                case BodyType.Neutron_Star:
                    return (int)Math.Round(NeutronStar + ((mass * NeutronStar) / StarFactor));
                case BodyType.White_Dwarf:
                    return (int)Math.Round(WhiteDwarf + ((mass * WhiteDwarf) / StarFactor));
                // Worlds
                // Other
                default:
                    return 0;
            }
        }
    }
}