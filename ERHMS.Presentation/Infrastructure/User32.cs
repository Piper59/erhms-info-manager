using System;
using System.Runtime.InteropServices;

namespace ERHMS.Presentation
{
    public static class User32
    {
        public const uint FLASHW_ALL = 0x03;
        public const uint FLASHW_CAPTION = 0x01;
        public const uint FLASHW_STOP = 0x00;
        public const uint FLASHW_TIMER = 0x04;
        public const uint FLASHW_TIMERNOFG = 0x0c;
        public const uint FLASHW_TRAY = 0x02;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        [DllImport("user32.dll")]
        public static extern int FlashWindowEx(ref FLASHWINFO pwfi);

        public static void FlashWindowEx(IntPtr hwnd, uint uCount)
        {
            FLASHWINFO pwfi = new FLASHWINFO
            {
                cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO))),
                hwnd = hwnd,
                dwFlags = FLASHW_ALL,
                uCount = uCount,
                dwTimeout = 0
            };
            FlashWindowEx(ref pwfi);
        }
    }
}
