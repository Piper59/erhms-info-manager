using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    // TODO: Allow access key
    public class IconButton : Button
    {
        public static readonly DependencyProperty AltTextProperty = DependencyProperty.Register(
            nameof(AltText),
            typeof(string),
            typeof(IconButton));

        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register(
            nameof(HelpText),
            typeof(string),
            typeof(IconButton));

        public string AltText
        {
            get { return (string)GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }

        public string HelpText
        {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }
    }
}
