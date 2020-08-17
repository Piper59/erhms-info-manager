using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ERHMS.Desktop.Controls
{
    [ContentProperty("Text")]
    public class WizardButton : Button
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(WizardButton));

        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register(
            nameof(HelpText),
            typeof(string),
            typeof(WizardButton));

        static WizardButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardButton), new FrameworkPropertyMetadata(typeof(WizardButton)));
            IsTabStopProperty.OverrideMetadata(typeof(WizardButton), new FrameworkPropertyMetadata(false));
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
