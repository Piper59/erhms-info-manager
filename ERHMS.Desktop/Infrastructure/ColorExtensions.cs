using System.Windows.Media;

namespace ERHMS.Desktop.Infrastructure
{
    public static class ColorExtensions
    {
        public static Color CopyWithAlpha(this Color @this, byte alpha)
        {
            return Color.FromArgb(alpha, @this.R, @this.G, @this.B);
        }
    }
}
