using System;
using System.Windows;
using System.Windows.Interop;

namespace ERHMS.Desktop.Infrastructure
{
    public static class WindowExtensions
    {
        public static IntPtr EnsureHandle(this Window @this)
        {
            WindowInteropHelper helper = new WindowInteropHelper(@this);
            return helper.EnsureHandle();
        }

        public static void SetOwner(this Window @this, Window owner)
        {
            owner.EnsureHandle();
            @this.Owner = owner;
            @this.WindowStartupLocation =
                owner.IsVisible ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
        }
    }
}
