using System.Linq;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public static class ApplicationExtensions
    {
        public static Window GetActiveWindow(this Application @this)
        {
            return @this.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
        }

        public static Window GetActiveOrMainWindow(this Application @this)
        {
            return @this.GetActiveWindow() ?? @this.MainWindow;
        }
    }
}
