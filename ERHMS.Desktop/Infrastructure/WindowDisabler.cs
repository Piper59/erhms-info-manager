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

        private bool oldIsEnabled;
        private Cursor oldOverrideCursor;

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
            oldIsEnabled = Window.IsEnabled;
            oldOverrideCursor = Mouse.OverrideCursor;
            Window.Closing += Window_Closing;
            Window.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void End()
        {
            Mouse.OverrideCursor = oldOverrideCursor;
            Window.IsEnabled = oldIsEnabled;
            Window.Closing -= Window_Closing;
        }

        void IDisposable.Dispose()
        {
            End();
        }
    }
}
