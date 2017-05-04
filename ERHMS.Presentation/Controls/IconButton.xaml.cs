using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public partial class IconButton : Button
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(UIElement),
            typeof(IconButton));
        public static readonly DependencyProperty AccessTextProperty = DependencyProperty.Register(
            "AccessText",
            typeof(string),
            typeof(IconButton));

        public UIElement Icon
        {
            get { return (UIElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string AccessText
        {
            get { return (string)GetValue(AccessTextProperty); }
            set { SetValue(AccessTextProperty, value); }
        }

        public IconButton()
        {
            InitializeComponent();
        }
    }
}
