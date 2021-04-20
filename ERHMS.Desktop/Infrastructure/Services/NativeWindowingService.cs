using ERHMS.Desktop.Services;
using System;
using System.Windows;
using System.Windows.Interop;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class NativeWindowingService : IWindowingService
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
            IntPtr handle = NativeMethods.GetActiveWindow();
            return handle == IntPtr.Zero ? null : Disabler.Begin(handle);
        }

        public bool? ShowDialog(Window window)
        {
            IntPtr handle = NativeMethods.GetActiveWindow();
            if (handle != IntPtr.Zero)
            {
                new WindowInteropHelper(window)
                {
                    Owner = handle
                };
            }
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return window.ShowDialog();
        }
    }
}
