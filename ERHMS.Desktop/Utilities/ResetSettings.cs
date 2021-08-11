using ERHMS.Common.Linq;
using ERHMS.Desktop.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public class ResetSettings : Utility.Headless
    {
        public bool Verbose { get; set; }

        public override IEnumerable<string> Parameters
        {
            get
            {
                yield return Verbose.ToString();
            }
            set
            {
                using (IEnumerator<string> enumerator = value.GetEnumerator())
                {
                    Verbose = bool.Parse(enumerator.GetNextOrDefault(bool.TrueString));
                }
            }
        }

        public override Task<string> ExecuteAsync()
        {
            Settings.Default.Reset();
            if (Verbose)
            {
                MessageBox.Show(Strings.Body_SettingsReset, Strings.Title_App);
            }
            return Task.FromResult((string)null);
        }
    }
}
