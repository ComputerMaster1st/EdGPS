using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDCurrentSystem.Models;

namespace EDCurrentSystem
{
    public class ConsoleOutput
    {
        private string NextSystemName = string.Empty;
        public StarSystem CurrentSystem { get; private set; } = new StarSystem("Waiting...", new List<double>() { 0, 0, 0 });

        public ConsoleOutput() {
            var loadedSystem = StarSystem.Load();
            if (!(loadedSystem is null)) CurrentSystem = loadedSystem;
        }

        public void SetNewSystem(string name, List<double> coordinates) {
            CurrentSystem = StarSystem.Load(name);
            if (CurrentSystem is null) CurrentSystem = new StarSystem(name, coordinates);            
            NextSystemName = string.Empty;
        }

        public void SetNextSystem(string name) {
            NextSystemName = name;
            CurrentSystem?.Save();
        }

        public void SendToConsole() {
            var found = 0;
            var bodies = CurrentSystem.Bodies.OrderBy(f => f.Id);
            var orbits = new StringBuilder();

            foreach (var body in bodies) {
                Branch.PrintBody(body, orbits, ref found, "", true);
            }

            double percentage =  0.00;
            var totalBodies = CurrentSystem.TotalBodies + CurrentSystem.TotalNonBodies;
            if (totalBodies > 0) {
                var division = found / (double)totalBodies;
                percentage = division * 100.00;
            }

            var summary = new StringBuilder()
                .AppendLine("Elite: Dangerous >> Galactic Positioning System || Created By: ComputerMaster1st")
                .AppendLine()
                .AppendFormat("Current System  : {0} {1}", CurrentSystem.Name, string.IsNullOrEmpty(NextSystemName) ? "" : $"=> Jumping To {NextSystemName}").AppendLine()
                .AppendFormat("Co-ordinates    : Latitude ({0}) | Longitude ({1}) | Elevation ({2})", CurrentSystem.Latitude, CurrentSystem.Longitude, CurrentSystem.Elevation).AppendLine()
                .AppendFormat("System Scan     : {0}% Complete {1}", Math.Round(percentage, 0), CurrentSystem.IsComplete ? "[System Scan Complete]" : "").AppendLine()
                .AppendFormat("Expected Bodies : {0} ({1} non-bodies) {2}", CurrentSystem.TotalBodies, CurrentSystem.TotalNonBodies, CurrentSystem.IsHonked ? "" : "[Awaiting FSS Disovery Scan]").AppendLine()
                .AppendLine("════════════════════════════════════════════════════════════════════════════════════");

            Console.Clear();
            Console.WriteLine(summary.ToString() + orbits.ToString());
        }
    }
}