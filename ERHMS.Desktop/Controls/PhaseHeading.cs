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
                Update();
            }
        }

        public PhaseHeading()
        {
            Margin = (Thickness)Application.Current.FindResource("SmallThickness");
            double padding = (double)Application.Current.FindResource("SmallSpace");
            Padding = new Thickness(padding, 0.0, padding, 0.0);
            Child = new TextBlock
            {
                Style = (Style)Application.Current.FindResource("Heading")
            };
        }

        private Color GetColor(byte alpha)
        {
            Color color = Phase.ToColor();
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private void Update()
        {
            Background = new LinearGradientBrush(GetColor(StartAlpha), GetColor(EndAlpha), StartPoint, EndPoint);
            Child.Text = Phase.ToDisplayName();
        }
    }
}
