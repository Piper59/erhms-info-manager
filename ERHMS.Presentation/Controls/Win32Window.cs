using System;
using System.Windows;
using System.Windows.Interop;

namespace ERHMS.Presentation.Controls
{
    public class Win32Window : System.Windows.Forms.IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public Win32Window(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            Handle = helper.Handle;
        }
    }
}
