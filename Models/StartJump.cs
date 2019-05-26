using System;
using Newtonsoft.Json;

namespace EDCurrentSystem.Models
{
    public class StartJump
    {
        public string JumpType { get; set; }
        public string SystemName { get; set; }

        [JsonConstructor]
        public StartJump(string jumpType, string starSystem) {
            JumpType = jumpType;
            SystemName = starSystem;
        }
    }
}