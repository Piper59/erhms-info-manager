using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public class Help : Utility
    {
        protected override bool LongRunning => false;

        protected override Task<string> RunCoreAsync()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("The following utilities are available:");
            foreach (string typeName in Types.Keys.OrderBy(typeName => typeName))
            {
                message.AppendLine();
                message.Append(typeName);
            }
            return Task.FromResult(message.ToString());
        }
    }
}
