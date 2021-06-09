using ERHMS.Common.Linq;
using ERHMS.Desktop.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public class ResetSettings : Utility.Headless
    {
        public bool Quiet { get; set; }

        public override IEnumerable<string> Parameters
        {
            get
            {
                yield return Quiet.ToString();
            }
            set
            {
                using (IEnumerator<string> enumerator = value.GetEnumerator())
                {
                    Quiet = bool.Parse(enumerator.GetNextOrDefault(bool.FalseString));
                }
            }
        }

        public override Task ExecuteAsync()
        {
            Settings.Default.Reset();
            if (!Quiet)
            {
                MessageBox.Show(Strings.Body_SettingsReset, Strings.Title_App);
            }
            return Task.CompletedTask;
        }
    }
}
