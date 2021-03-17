using System;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure
{
    public class WindowDisabler : IDisposable
    {
        private class State
        {
            public Window Window { get; }
            public bool Enabled { get; }
            public Cursor Cursor { get; }

            public State(Window window)
            {
                Window = window;
                Enabled = window.IsEnabled;
                Cursor = Mouse.OverrideCursor;
            }

            public void Restore()
            {
                Mouse.OverrideCursor = Cursor;
                Window.IsEnabled = Enabled;
            }
        }

        public static WindowDisabler Begin(Window window)
        {
            WindowDisabler disabler = new WindowDisabler(window);
            disabler.Begin();
            return disabler;
        }

        private State state;

        public Window Window { get; }

        public WindowDisabler(Window window)
        {
            Window = window;
        }

        public void Begin()
        {
            if (state != null)
            {
                throw new InvalidOperationException("Previous operation is not complete.");
            }
            state = new State(Window);
            Window.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void End()
        {
            state?.Restore();
            state = null;
        }

        public void Dispose()
        {
            End();
        }
    }
}
