﻿using System;
using System.Runtime.InteropServices;

namespace ERHMS.Desktop.Infrastructure
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }
}
