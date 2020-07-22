using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ERHMS.Desktop.Controls
{
    public class PhaseHeading : Border
    {
        private const byte StartAlpha = 0x40;
        private const byte EndAlpha = 0x00;
        private static readonly Point StartPoint = new Point(0.5, 0.0);
        private static readonly Point EndPoint = new Point(1.0, 0.0);

        private static Color GetColor(Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private static Brush GetBackground(Color color)
        {
            Color startColor = GetColor(color, StartAlpha);
            Color endColor = GetColor(color, EndAlpha);
            return new LinearGradientBrush(startColor, endColor, StartPoint, EndPoint);
        }

        public new TextBlock Child
        {
            get { return (TextBlock)base.Child; }
            set { base.Child = value; }
        }

        private Phase phase;
        public Phase Phase
        {
            get
            {
                return phase;
            }
            set
            {
                phase = value;
                Background = GetBackground(value.ToColor());
                Child.Text = value.ToDisplayName();
            }
        }

        public PhaseHeading()
        {
            Margin = (Thickness)Application.Current.FindResource("SmallThickness");
            double padding = (double)Application.Current.FindResource("SmallSpace");
            Padding = new Thickness(padding, 0.0, padding, 0.0);
            SnapsToDevicePixels = true;
            Child = new TextBlock
            {
                Style = (Style)Application.Current.FindResource("Heading")
            };
        }
    }
}
