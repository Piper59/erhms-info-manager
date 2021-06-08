using ERHMS.Desktop.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public class ResetSettings : IUtility
    {
        public string Instructions => null;
        public IEnumerable<string> Parameters { get; set; }

        public Task<string> ExecuteAsync()
        {
            Settings.Default.Reset();
            MessageBox.Show(Strings.Body_SettingsReset, Strings.Title_App);
            return Task.FromResult((string)null);
        }
    }
}
