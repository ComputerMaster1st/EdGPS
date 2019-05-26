using System.Linq;
using System.Text;
using EDCurrentSystem.Models;

namespace EDCurrentSystem
{
    public class Branch
    {
        private const string _cross = " ├─";
        private const string _corner = " └─";
        private const string _vertical = " │ ";
        private const string _space = "   ";

        public static void PrintBody(Body body, StringBuilder output, ref int found, string indent, bool isLast) {
            output.AppendFormat(indent);

            if (isLast) {
                output.AppendFormat(_corner);
                indent += _space;
            } else {
                output.AppendFormat(_cross);
                indent += _vertical;
            }

            var discovered = body.Discovered ? "" : " [Discovered]";

            if (body.Type == BodyType.Star) {
                output.AppendFormat("{0} [{1} Class] [{2} ls]{3}",
                    body.Name,
                    body.SubType, 
                    body.Distance,
                    discovered
                ).AppendLine();
                found++;
            } else if (body.Type == BodyType.Belt) {
                output.AppendFormat("{0} [{1} ls]{2}",
                    body.Name,
                    body.Distance,
                    discovered
                ).AppendLine();
                found++;
            } else if (body.Type == BodyType.Planet) {
                output.AppendFormat("{0} [{1} World] [{2} ls]{3}{4}{5}",
                    body.Name,
                    body.SubType, 
                    body.Distance,
                    discovered,
                    body.Mapped ? " [Is Mapped]" : (body.IsDssScanned ? " [DSS Complete]" : ""),
                    string.IsNullOrWhiteSpace(body.Terraformable) ? "" : " [Terraformable]"
                ).AppendLine();
                found++;
            } else {
                output.AppendFormat("(x)").AppendLine();
            }

            var numberOfChildren = body.SubBodies.Count;
            var orderedSubBodies = body.SubBodies.OrderBy(f => f.Id).ToList();
            for (var i = 0; i < numberOfChildren; i++) {
                var child = orderedSubBodies[i];
                var isLast2 = (i == (numberOfChildren - 1));
                PrintBody(child, output, ref found, indent, isLast2);
            }
        }
    }
}