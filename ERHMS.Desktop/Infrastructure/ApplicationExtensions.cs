using System;
using System.Windows;
using System.Windows.Interop;

namespace ERHMS.Desktop.Infrastructure
{
    public static class ApplicationExtensions
    {
        public static Window GetActiveWindow(this Application @this)
        {
            IntPtr handle = NativeMethods.GetActiveWindow();
            foreach (Window window in @this.Windows)
            {
                WindowInteropHelper helper = new WindowInteropHelper(window);
                if (helper.Handle == handle)
                {
                    return window;
                }
            }
            foreach (Window window in @this.Windows)
            {
                if (window.IsActive)
                {
                    return window;
                }
            }
            return @this.MainWindow;
        }
    }
}
