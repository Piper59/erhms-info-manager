using ERHMS.Desktop.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class WpfWindowingService : IWindowingService
    {
        private class Disabler : IDisposable
        {
            public static Disabler Begin(Window window)
            {
                Disabler disabler = new Disabler(window);
                disabler.Begin();
                return disabler;
            }

            public Window Window { get; }

            private Disabler(Window window)
            {
                Window = window;
            }

            private void Window_Closing(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
            }

            private void Begin()
            {
                Window.Closing += Window_Closing;
                Window.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;
            }

            public void End()
            {
                Mouse.OverrideCursor = null;
                Window.IsEnabled = true;
                Window.Closing -= Window_Closing;
            }

            void IDisposable.Dispose()
            {
                End();
            }
        }

        public IDisposable Disable()
        {
            return Disabler.Begin(Application.Current.MainWindow);
        }

        public bool? ShowDialog(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window.ShowDialog();
        }
    }
}
