using System;
using System.Windows;

namespace ERHMS.Desktop.Services
{
    public interface IWindowingService
    {
        IDisposable Disable();
        bool? ShowDialog(Window window);
    }
}
