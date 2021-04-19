using ERHMS.Desktop.Services;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class WinFormsWindowingService : IWindowingService
    {
        private class Disabler : IDisposable
        {
            public static Disabler Begin(IntPtr handle)
            {
                Disabler disabler = new Disabler(handle);
                disabler.Begin();
                return disabler;
            }

            public IntPtr Handle { get; }

            private Disabler(IntPtr handle)
            {
                Handle = handle;
            }

            private void Begin()
            {
                NativeMethods.EnableWindow(Handle, false);
            }

            public void End()
            {
                NativeMethods.EnableWindow(Handle, true);
            }

            void IDisposable.Dispose()
            {
                End();
            }
        }

        public IDisposable Disable()
        {
            return Disabler.Begin(Form.ActiveForm.Handle);
        }

        public bool? ShowDialog(Window window)
        {
            new WindowInteropHelper(window)
            {
                Owner = Form.ActiveForm.Handle
            };
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return window.ShowDialog();
        }
    }
}
