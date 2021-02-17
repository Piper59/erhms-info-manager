using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ERHMS.Desktop.Controls
{
    [ContentProperty(nameof(Text))]
    public class VerboseButton : Button
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(VerboseButton));

        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register(
            nameof(HelpText),
            typeof(string),
            typeof(VerboseButton));

        static VerboseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(VerboseButton),
                new FrameworkPropertyMetadata(typeof(VerboseButton)));
            IsTabStopProperty.OverrideMetadata(typeof(VerboseButton), new FrameworkPropertyMetadata(false));
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string HelpText
        {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }
    }
}
