using System;
using System.Windows;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace ERHMS.Presentation.Controls
{
    public class Win32Window : IWin32Window
    {
        private WindowInteropHelper helper;

        public IntPtr Handle
        {
            get { return helper.Handle; }
        }

        public Win32Window(Window window)
        {
            helper = new WindowInteropHelper(window);
        }
    }
}
