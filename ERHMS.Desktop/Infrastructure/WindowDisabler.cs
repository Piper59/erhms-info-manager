using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure
{
    public class WindowDisabler : IDisposable
    {
        public static WindowDisabler Begin(Window window)
        {
            if (!window.IsVisible)
            {
                return null;
            }
            WindowDisabler disabler = new WindowDisabler(window);
            disabler.Begin();
            return disabler;
        }

        public Window Window { get; }

        private WindowDisabler(Window window)
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
}
