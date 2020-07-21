using ERHMS.Desktop.Properties;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public class ResetSettings : Utility
    {
        public override bool LongRunning => false;

        protected override Task<string> RunCoreAsync()
        {
            Settings.Default.Reset();
            return Task.FromResult("Settings have been reset.");
        }
    }
}
