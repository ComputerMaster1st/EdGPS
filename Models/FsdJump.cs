using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EDCurrentSystem.Models
{
    public class FsdJump
    {
        public string SystemName { get; set; }
        public List<double> SystemPosition { get; set; }
        public double JumpDistanceTaken { get; set; }
        public double FuelUsed { get; set; }
        public double CurrentFuelLevel { get; set; }

        [JsonConstructor]
        public FsdJump(string starSystem, List<double> starPos, double jumpDist, double fuelUsed, double fuelLevel) {
            SystemName = starSystem;
            SystemPosition = starPos;
            JumpDistanceTaken = jumpDist;
            FuelUsed = fuelUsed;
            CurrentFuelLevel = fuelLevel;
        }
    }
}