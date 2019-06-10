using System;
using System.Linq;
using System.Text;
using EdGps.Core;
using EdGps.Core.Models;

namespace EdGps
{
    public class ConsoleWriter
    {
        private const string _cross = " ├─";
        private const string _corner = " └─";
        private const string _vertical = " │ ";
        private const string _space = "   ";

        public void Write(StarSystem system, string nextSystem = null) {
            var found = 0;
            var bodies = system.Bodies.OrderBy(f => f.Id);
            var orbits = new StringBuilder();

            foreach (var body in bodies)
                PrintBody(system.Name, body, orbits, ref found, "", true);

            Console.Clear();
            Console.WriteLine(CreateHeader(system, found, nextSystem) + orbits.ToString());
        }

        private string AddBrackets(string input) => $" ■ {input}";

        private string CreateHeader(StarSystem system, int found, string nextSystem = null) {
            double percentage =  0.00;
            var totalBodies = system.TotalBodies + system.TotalNonBodies;
            if (totalBodies > 0) {
                var division = found / (double)totalBodies;
                percentage = division * 100.00;
            }

            var output = new StringBuilder()
                .AppendLine("Elite: Dangerous >> Galactic Positioning System || Created By: ComputerMaster1st")
                .AppendLine()
                .AppendFormat("Current System  : {0} {1}", system.Name, string.IsNullOrEmpty(nextSystem) ? "" : $"=> Jumping To {nextSystem}").AppendLine()
                .AppendFormat("Co-ordinates    : Latitude ({0}) | Longitude ({1}) | Elevation ({2})", system.Latitude, system.Longitude, system.Elevation).AppendLine()
                .AppendFormat("Distance (Ly)   : {0} (Sol)", string.Format("{0:n0}", Math.Round(Math.Pow(Math.Pow(system.Longitude, 2) + 
                    Math.Pow(system.Latitude, 2) + Math.Pow(system.Elevation, 2), 0.5), 2))).AppendLine()
                .AppendFormat("System Scan     : {0}% Complete{1}", Math.Round(percentage, 0), system.IsComplete ? AddBrackets("System Scan Complete") : "").AppendLine()
                .AppendFormat("Expected Bodies : {0} ({1} non-bodies){2}", system.TotalBodies, system.TotalNonBodies, system.IsHonked ? "" : AddBrackets("Awaiting FSS Discovery Scan")).AppendLine()
                .AppendLine("════════════════════════════════════════════════════════════════════════════════════════════════════");

            return output.ToString();
        }

        private void PrintBody(string systemName, Body body, StringBuilder output, ref int found, string indent, bool isLast) {
            output.AppendFormat(indent);

            if (isLast) {
                output.AppendFormat(_corner);
                indent += _space;
            } else {
                output.AppendFormat(_cross);
                indent += _vertical;
            }

            var discovered = body.Discovered ? "" : AddBrackets("Discovered");
            var bodyName = body.Name.Replace(systemName, "");
            var distance = string.Format("{0:n0}", body.Distance);

            switch (body.Type) {
                
                case BodyType.Belt:
                    output.AppendFormat("{0}{1}{2}",
                        bodyName,
                        AddBrackets($"{distance} ls"),
                        discovered
                    ).AppendLine();
                    found++;
                    break;
                case BodyType.Planet:
                    output.AppendFormat("{0}{1}{2}{3}{4}{5}",
                        bodyName,
                        AddBrackets($"{body.SubType}"),
                        AddBrackets($"{distance} ls"),
                        discovered,
                        body.Mapped ? AddBrackets("Is Mapped") : (body.IsDssScanned ? AddBrackets("DSS Complete") : ""),
                        string.IsNullOrWhiteSpace(body.Terraformable) ? "" : AddBrackets("Terraformable")
                    ).AppendLine();
                    found++;
                    break;
                case BodyType.Star:
                    output.AppendFormat("{0}{1}{2}{3}{4}",
                        body.Name,
                        AddBrackets($"Class {body.SubType}"),
                        AddBrackets($"{distance} ls"),
                        AddBrackets($"{BodyValue.GetStarValue(body.Type, body.Mass)} cr"),
                        discovered
                    ).AppendLine();
                    found++;
                    break;
                case BodyType.White_Dwarf:
                    output.AppendFormat("{0}{1}{2}{3}",
                        body.Name,
                        AddBrackets($"White Dwarf ({body.SubType})"),
                        AddBrackets($"{distance} ls"),
                        discovered
                    ).AppendLine();
                    found++;
                    break;
                case BodyType.Black_Hole:
                case BodyType.Neutron_Star:
                case BodyType.T_Tauri_Star:
                    output.AppendFormat("{0}{1}{2}{3}",
                        body.Name,
                        AddBrackets(body.Type.ToString().Replace('_', ' ')),
                        AddBrackets($"{distance} ls"),
                        discovered
                    ).AppendLine();
                    found++;
                    break;
                default:
                    output.AppendFormat("(x)").AppendLine();
                    break;
            }

            var numberOfChildren = body.SubBodies.Count;
            var orderedSubBodies = body.SubBodies.OrderBy(f => f.Id).ToList();
            for (var i = 0; i < numberOfChildren; i++) {
                var child = orderedSubBodies[i];
                var isLast2 = (i == (numberOfChildren - 1));
                PrintBody(systemName, child, output, ref found, indent, isLast2);
            }
        }
    }
}