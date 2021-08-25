using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace ERHMS.Desktop.EventHandlers
{
    public static class OpenUriOnRequestNavigate
    {
        public static void Register()
        {
            EventManager.RegisterClassHandler(
                typeof(Hyperlink),
                Hyperlink.RequestNavigateEvent,
                new RequestNavigateEventHandler(Hyperlink_RequestNavigate));
        }

        private static void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString())?.Dispose();
            e.Handled = true;
        }
    }
}
