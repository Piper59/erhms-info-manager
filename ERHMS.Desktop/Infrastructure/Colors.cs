using System.Windows.Media;

namespace ERHMS.Desktop.Infrastructure
{
    internal static class Colors
    {
        public static readonly Color ThemeBlue = Color.FromRgb(0x00, 0x79, 0xc1);
        public static readonly Color ThemeRed = Color.FromRgb(0xef, 0x3e, 0x42);
        public static readonly Color ThemeGreen = Color.FromRgb(0x6c, 0xb3, 0x3f);
        public static readonly Color PreDeployment = ThemeBlue;
        public static readonly Color Deployment = ThemeRed;
        public static readonly Color PostDeployment = ThemeGreen;
        public static readonly Color Accent = ThemeBlue;
    }
}
