using System.Linq;
using System.Text;

namespace ERHMS.Console.Utilities
{
    public class Help : Utility
    {
        public static string GetUsage()
        {
            StringBuilder usage = new StringBuilder();
            usage.Append("The following utilities are available:");
            foreach (string typeName in Types.Keys.OrderBy(typeName => typeName))
            {
                usage.AppendLine();
                usage.Append($"  {typeName}");
            }
            return usage.ToString();
        }

        protected override void RunCore()
        {
            Out.WriteLine(GetUsage());
        }
    }
}
