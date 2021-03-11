using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure
{
    public static class WindowExtensions
    {
        private class DialogShower : IDisposable
        {
            private bool closing;
            private Exception exception;

            public Window Window { get; }

            public DialogShower(Window window)
            {
                Window = window;
                window.Closing += Window_Closing;
                SynchronizationContext.Current.Post(_ => Show(), null);
            }

            private void Window_Closing(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
            }

            private void Show()
            {
                if (closing)
                {
                    return;
                }
                try
                {
                    Window.ShowDialog();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void Close()
            {
                closing = true;
                Window.Closing -= Window_Closing;
                Window.Close();
                if (exception != null)
                {
                    throw new AggregateException(exception);
                }
            }

            public void Dispose()
            {
                Close();
            }
        }

        private class Disabler : IDisposable
        {
            private readonly bool oldIsEnabled;
            private readonly Cursor oldOverrideCursor;

            public Window Window { get; }

            public Disabler(Window window)
            {
                Window = window;
                oldIsEnabled = window.IsEnabled;
                oldOverrideCursor = Mouse.OverrideCursor;
                window.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;
            }

            public void Dispose()
            {
                Mouse.OverrideCursor = oldOverrideCursor;
                Window.IsEnabled = oldIsEnabled;
            }
        }

        public static IDisposable BeginDisable(this Window @this)
        {
            return new Disabler(@this);
        }

        public static IDisposable BeginShowDialog(this Window @this)
        {
            return new DialogShower(@this);
        }
    }
}
