using System.Diagnostics;
using System.Windows.Documents;

namespace ERHMS.Presentation.Controls
{
    public class WebHyperlink : Hyperlink
    {
        public WebHyperlink()
        {
            RequestNavigate += (sender, e) =>
            {
                Process.Start(e.Uri.AbsoluteUri);
            };
        }
    }
}
