using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Infrastructure
{
    public class NonBlockingModalDialog
    {
        private class Shower : IDisposable
        {
            public static Shower Begin(Window window)
            {
                Shower shower = new Shower(window);
                shower.Begin();
                return shower;
            }

            private DispatcherOperation operation;
            private bool closed;

            public Window Window { get; }

            public Shower(Window window)
            {
                Window = window;
            }

            private void Window_Closing(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
            }

            public void Begin()
            {
                if (operation != null)
                {
                    throw new InvalidOperationException("Previous operation is not complete.");
                }
                operation = Window.Dispatcher.InvokeAsync(() =>
                {
                    if (operation != null && closed)
                    {
                        return;
                    }
                    Window.Closing += Window_Closing;
                    Window.ShowDialog();
                });
            }

            public void End()
            {
                Window.Closing -= Window_Closing;
                Window.Close();
                closed = true;
            }

            public void Dispose()
            {
                End();
            }

            public TaskAwaiter GetAwaiter()
            {
                TaskAwaiter awaiter = operation.GetAwaiter();
                operation = null;
                closed = false;
                return awaiter;
            }
        }

        private Shower shower;

        public Window Window { get; }

        public NonBlockingModalDialog(Window window)
        {
            Window = window;
        }

        public IDisposable BeginShow()
        {
            if (shower != null)
            {
                throw new InvalidOperationException("Previous operation is not complete.");
            }
            shower = Shower.Begin(Window);
            return shower;
        }

        public async Task EndShowAsync()
        {
            if (shower != null)
            {
                await shower;
            }
        }
    }
}
