using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ERHMS.Presentation.Controls
{
    public class AppBarButton : Button
    {
        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register(
            "Visual",
            typeof(Visual),
            typeof(AppBarButton));
        public Visual Visual
        {
            get { return (Visual)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }
    }
}
