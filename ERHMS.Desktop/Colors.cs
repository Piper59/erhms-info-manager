using System.Windows.Media;

namespace ERHMS.Desktop
{
    public static class Colors
    {
        public static Color ThemeBlue { get; } = Color.FromRgb(0x00, 0x79, 0xc1);
        public static Color ThemeRed { get; } = Color.FromRgb(0xef, 0x3e, 0x42);
        public static Color ThemeGreen { get; } = Color.FromRgb(0x6c, 0xb3, 0x3f);
        public static Color PreDeployment => ThemeBlue;
        public static Color Deployment => ThemeRed;
        public static Color PostDeployment => ThemeGreen;
    }
}
