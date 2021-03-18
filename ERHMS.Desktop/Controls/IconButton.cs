using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public class IconButton : Button
    {
        public static readonly DependencyProperty AltTextProperty = DependencyProperty.Register(
            nameof(AltText),
            typeof(string),
            typeof(IconButton));

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(IconButton),
                new FrameworkPropertyMetadata(typeof(IconButton)));
            IsTabStopProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(false));
        }

        public string AltText
        {
            get { return (string)GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }
    }
}
