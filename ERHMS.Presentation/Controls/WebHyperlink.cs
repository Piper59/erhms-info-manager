using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace ERHMS.Presentation.Controls
{
    public class WebHyperlink : Hyperlink
    {
        public WebHyperlink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
