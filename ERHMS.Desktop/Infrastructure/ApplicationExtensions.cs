using System.Linq;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public static class ApplicationExtensions
    {
        public static Window GetActiveWindow(this Application @this)
        {
            return @this.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive) ?? @this.MainWindow;
        }
    }
}
