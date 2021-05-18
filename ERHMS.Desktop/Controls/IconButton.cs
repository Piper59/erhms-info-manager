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

        public static readonly DependencyProperty AlternativeTextProperty = DependencyProperty.Register(
            nameof(AlternativeText),
            typeof(string),
            typeof(IconButton));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(IconButton));

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

        public string AlternativeText
        {
            get { return (string)GetValue(AlternativeTextProperty); }
            set { SetValue(AlternativeTextProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
    }
}
