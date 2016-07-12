using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ERHMS.Presentation.Controls
{
    public partial class AppBarButton : Button
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Visual),
            typeof(AppBarButton));
        public Visual Icon
        {
            get { return (Visual)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public AppBarButton()
        {
            InitializeComponent();
        }
    }
}
