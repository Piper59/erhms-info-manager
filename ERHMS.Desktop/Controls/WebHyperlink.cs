using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace ERHMS.Desktop.Controls
{
    public class WebHyperlink : Hyperlink
    {
        public WebHyperlink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString())?.Dispose();
        }
    }
}
