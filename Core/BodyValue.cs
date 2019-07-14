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

        public static (int Value, int HonkValue) GetBodyValue(Body body) {
            switch (body.Type) {
                // Stars
                case BodyType.Star:
                case BodyType.Proto_Star:
                case BodyType.Wolf_Rayet:
                case BodyType.Giant:
                case BodyType.Super_Giant:
                case BodyType.Carbon_Star:
                    return GetStarValue(Star, body);
                case BodyType.Black_Hole:
                    return GetStarValue(BlackHole, body);
                case BodyType.Neutron_Star:
                    return GetStarValue(NeutronStar, body);
                case BodyType.White_Dwarf:
                    return GetStarValue(WhiteDwarf, body);

                // Worlds
                case BodyType.AmmoniaWorld:
                    return GetWorldValue(AmmoniaWorld, body);
                case BodyType.EarthlikeWorld:
                    return GetWorldValue(EarthlikeWorld, body);
                case BodyType.GasGiant:
                    return GetWorldValue(GasGiant, body);
                case BodyType.GasGiant2:
                    return GetWorldValue(GasGiantClass2, body);
                case BodyType.HighMetalContent:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) return GetWorldValue(TerraformableHighMetalContent, body);
                    return GetWorldValue(HighMetalContent, body);
                case BodyType.MetalRich:
                    return GetWorldValue(MetalRich, body);
                case BodyType.WaterWorld:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) return GetWorldValue(TerraformableWaterWorld, body);
                    return GetWorldValue(WaterWorld, body);
                case BodyType.Planet:
                    if (!string.IsNullOrWhiteSpace(body.Terraformable)) return GetWorldValue(TerraformableOther, body);
                    return GetWorldValue(Other, body);

                // Other/Non-Bodies
                default:
                    return (0, 0);
            }
        }

        private static (int Value, int HonkValue) GetStarValue(int baseValue, Body body) {
            var value = (int)Math.Round(baseValue + (body.Mass * baseValue / StarFactor));
            var honkValue = (int)Math.Round(baseValue / 3 * (body.Discovered ? 1 : 2.6));
            return (value, honkValue);
        }

        private static (int Value, int HonkValue) GetWorldValue(int baseValue, Body body) {
            var mappingMultiplier = 1.0;
            var isFirstDiscovered = !body.Discovered;
            var isFirstMapped = !body.Mapped && body.IsDssScanned ? true : false;

            if (body.Mapped && body.IsDssScanned || isFirstMapped) {
                if (isFirstDiscovered && isFirstMapped) mappingMultiplier = 3.699622554;
                else if (isFirstMapped) mappingMultiplier = 8.0956;
                else mappingMultiplier = 3.3333333333;

                mappingMultiplier *= (body.DssEfficiencyAchieved) ? 1.25 : 1;
            }

            var value = Math.Max(500, (baseValue + (baseValue * WorldFactor * Math.Pow(body.Mass, 0.2))) * mappingMultiplier);
            var honkValue = Math.Max(500, (baseValue + (baseValue * WorldFactor * Math.Pow(body.Mass, 0.2))) / 3);
            value *= (isFirstDiscovered) ? 2.6 : 1;
            honkValue *= (isFirstDiscovered) ? 2.6 : 1;

            return ((int)Math.Round(value), (int)Math.Round(honkValue));
        }
    }
}