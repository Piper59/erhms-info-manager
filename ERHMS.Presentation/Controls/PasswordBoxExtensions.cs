using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public static class PasswordBoxExtensions
    {
        public static readonly DependencyProperty BoundProperty = DependencyProperty.RegisterAttached(
            "Bound",
            typeof(bool),
            typeof(PasswordBoxExtensions),
            new PropertyMetadata(Bound_PropertyChanged));
        public static bool GetBound(PasswordBox target)
        {
            return (bool)target.GetValue(BoundProperty);
        }
        public static void SetBound(PasswordBox target, bool value)
        {
            target.SetValue(BoundProperty, value);
        }

        public static readonly DependencyProperty UpdatingProperty = DependencyProperty.RegisterAttached(
            "Updating",
            typeof(bool),
            typeof(PasswordBoxExtensions));
        public static bool GetUpdating(PasswordBox target)
        {
            return (bool)target.GetValue(UpdatingProperty);
        }
        public static void SetUpdating(PasswordBox target, bool value)
        {
            target.SetValue(UpdatingProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxExtensions),
            new PropertyMetadata(Password_PropertyChanged));
        public static string GetPassword(PasswordBox target)
        {
            return (string)target.GetValue(PasswordProperty);
        }
        public static void SetPassword(PasswordBox target, string value)
        {
            target.SetValue(PasswordProperty, value);
        }

        private static void Bound_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox target = (PasswordBox)sender;
            SetUpdating(target, true);
            if ((bool)e.OldValue)
            {
                target.PasswordChanged -= Bound_Target_PasswordChanged;
                SetPassword(target, null);
            }
            if ((bool)e.NewValue)
            {
                target.PasswordChanged += Bound_Target_PasswordChanged;
                SetPassword(target, target.Password);
            }
            SetUpdating(target, false);
        }

        private static void Bound_Target_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox target = (PasswordBox)sender;
            if (!GetUpdating(target))
            {
                SetUpdating(target, true);
                SetPassword(target, target.Password);
                SetUpdating(target, false);
            }
        }

        private static void Password_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox target = (PasswordBox)sender;
            if (!GetUpdating(target))
            {
                SetUpdating(target, true);
                target.Password = (string)e.NewValue;
                SetUpdating(target, false);
            }
        }
    }
}
