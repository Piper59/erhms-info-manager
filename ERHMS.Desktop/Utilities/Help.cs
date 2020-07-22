using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public class Help : Utility
    {
        public override bool LongRunning => false;

        protected override Task<string> RunCoreAsync()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("The following utilities are available:");
            foreach (string typeName in Types.Keys.OrderBy(typeName => typeName))
            {
                result.AppendLine();
                result.Append($"    \u2022 {typeName}");
            }
            return Task.FromResult(result.ToString());
        }
    }
}
