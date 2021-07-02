using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public static class ThicknessExtensions
    {
        public static Thickness Scale(
            this Thickness @this,
            double left = 1.0,
            double top = 1.0,
            double right = 1.0,
            double bottom = 1.0)
        {
            return new Thickness(@this.Left * left, @this.Top * top, @this.Right * right, @this.Bottom * bottom);
        }
    }
}
