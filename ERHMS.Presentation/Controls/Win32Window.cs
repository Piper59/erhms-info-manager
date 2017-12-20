using System;
using System.Windows;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace ERHMS.Presentation.Controls
{
    public class Win32Window : IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public Win32Window(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            Handle = helper.Handle;
        }
    }
}
