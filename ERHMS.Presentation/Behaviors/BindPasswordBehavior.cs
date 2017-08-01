using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Behaviors
{
    public class BindPasswordBehavior : BridgeBindingBehavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(BindPasswordBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PasswordProperty_PropertyChanged));

        private static void PasswordProperty_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindPasswordBehavior)sender).Push();
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }

        private void Push()
        {
            Update(() =>
            {
                AssociatedObject.Password = Password;
            });
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Update(() =>
            {
                Password = AssociatedObject.Password;
            });
        }
    }
}
