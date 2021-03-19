using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Controls
{
    public class IconButton : Button
    {
        public static readonly DependencyProperty AccessKeyProperty = DependencyProperty.Register(
            nameof(AccessKey),
            typeof(string),
            typeof(IconButton),
            new FrameworkPropertyMetadata(AccessKeyProperty_DependencyPropertyChanged));

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

        private static void AccessKeyProperty_DependencyPropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            IInputElement element = (IInputElement)sender;
            if (e.OldValue is string oldKey)
            {
                AccessKeyManager.Unregister(oldKey, element);
            }
            if (e.NewValue is string newKey)
            {
                AccessKeyManager.Register(newKey, element);
            }
        }

        public string AccessKey
        {
            get { return (string)GetValue(AccessKeyProperty); }
            set { SetValue(AccessKeyProperty, value); }
        }

        public string AltText
        {
            get { return (string)GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }
    }
}
