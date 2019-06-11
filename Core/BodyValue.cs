using EdGps.Core.Models;
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

        public static int GetBodyValue(Body body) {
            int value;
            switch (body.Type) {
                // Stars
                case BodyType.Star:
                    value = (int)Math.Round(Star + ((body.Mass * Star) / StarFactor));
                    break;
                case BodyType.Black_Hole:
                    value = (int)Math.Round(BlackHole + ((body.Mass * BlackHole) / StarFactor));
                    break;
                case BodyType.Neutron_Star:
                    value = (int)Math.Round(NeutronStar + ((body.Mass * NeutronStar) / StarFactor));
                    break;
                case BodyType.White_Dwarf:
                    value = (int)Math.Round(WhiteDwarf + ((body.Mass * WhiteDwarf) / StarFactor));
                    break;

                // Worlds
                case BodyType.AmmoniaWorld:
                    value = GetWorldValue(AmmoniaWorld, body);
                    break;
                case BodyType.EarthlikeWorld:
                    value = GetWorldValue(EarthlikeWorld, body);
                    break;
                case BodyType.GasGiant:
                    value = GetWorldValue(GasGiant, body);
                    break;
                case BodyType.GasGiant2:
                    value = GetWorldValue(GasGiantClass2, body);
                    break;
                case BodyType.HighMetalContent:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) {
                        value = GetWorldValue(TerraformableHighMetalContent, body);
                        break;
                    }

                    value = GetWorldValue(HighMetalContent, body);
                    break;
                case BodyType.MetalRich:
                    value = GetWorldValue(MetalRich, body);
                    break;
                case BodyType.WaterWorld:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) {
                        value = GetWorldValue(TerraformableWaterWorld, body);
                        break;
                    }

                    value = GetWorldValue(WaterWorld, body);
                    break;
                case BodyType.Planet:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) {
                        value = GetWorldValue(TerraformableOther, body);
                        break;
                    }

                    value = GetWorldValue(Other, body);
                    break;

                // Other/Non-Bodies
                default:
                    value = 0;
                    break;
            }

            return Math.Max(500, value);
        }

        private static int GetWorldValue(int baseValue, Body body) {
            var mappingMultiplier = 1.0;
            var isFirstDiscovered = !body.Discovered;
            var isFirstMapped = !body.Mapped && body.IsDssScanned ? true : false;

            if (body.Mapped) {
                if (isFirstDiscovered && isFirstMapped) mappingMultiplier = 3.699622554;
                else if (isFirstMapped) mappingMultiplier = 8.0956;
                else mappingMultiplier = 3.3333333333;

                mappingMultiplier *= (body.DssEfficiencyAchieved) ? 1.25 : 1;
            }

            var value = Math.Max(500, (baseValue + (baseValue * WorldFactor * Math.Pow(body.Mass, 0.2))) * mappingMultiplier);
            value *= (isFirstDiscovered) ? 2.6 : 1;
            return (int)Math.Round(value);
        }
    }
}