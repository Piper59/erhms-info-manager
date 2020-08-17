using ERHMS.Domain;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public class PhaseHeading : Control
    {
        public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(
            nameof(Phase),
            typeof(Phase),
            typeof(PhaseHeading));

        static PhaseHeading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PhaseHeading), new FrameworkPropertyMetadata(typeof(PhaseHeading)));
        }

        public Phase Phase
        {
            get { return (Phase)GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }
    }
}
