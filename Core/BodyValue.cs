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

        public static int GetBodyValue(BodyType type, double mass, bool isTerraformable) {
            int value;
            switch (type) {
                // Stars
                case BodyType.Star:
                    value = (int)Math.Round(Star + ((mass * Star) / StarFactor));
                    break;
                case BodyType.Black_Hole:
                    value = (int)Math.Round(BlackHole + ((mass * BlackHole) / StarFactor));
                    break;
                case BodyType.Neutron_Star:
                    value = (int)Math.Round(NeutronStar + ((mass * NeutronStar) / StarFactor));
                    break;
                case BodyType.White_Dwarf:
                    value = (int)Math.Round(WhiteDwarf + ((mass * WhiteDwarf) / StarFactor));
                    break;

                // Worlds
                case BodyType.AmmoniaWorld:
                    value = (int)Math.Round(AmmoniaWorld + ((AmmoniaWorld * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.EarthlikeWorld:
                    value = (int)Math.Round(EarthlikeWorld + ((EarthlikeWorld * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.GasGiant:
                    value = (int)Math.Round(GasGiant + ((GasGiant * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.GasGiant2:
                    value = (int)Math.Round(GasGiantClass2 + ((GasGiantClass2 * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.HighMetalContent:
                    if (isTerraformable) {
                        value = (int)Math.Round(TerraformableHighMetalContent + ((TerraformableHighMetalContent * WorldFactor) * Math.Pow(mass, 0.2)));
                        break;
                    }

                    value = (int)Math.Round(HighMetalContent + ((HighMetalContent * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.MetalRich:
                    value = (int)Math.Round(MetalRich + ((MetalRich * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.WaterWorld:
                    if (isTerraformable) {
                        value = (int)Math.Round(TerraformableWaterWorld + ((TerraformableWaterWorld * WorldFactor) * Math.Pow(mass, 0.2)));
                        break;
                    }

                    value = (int)Math.Round(WaterWorld + ((WaterWorld * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;
                case BodyType.Planet:
                    if (isTerraformable) {
                        value = (int)Math.Round(TerraformableOther + ((TerraformableOther * WorldFactor) * Math.Pow(mass, 0.2)));
                        break;
                    }

                    value = (int)Math.Round(Other + ((Other * WorldFactor) * Math.Pow(mass, 0.2)));
                    break;

                // Other/Non-Bodies
                default:
                    value = 0;
                    break;
            }

            return Math.Max(500, value);
        }
    }
}