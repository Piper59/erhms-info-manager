using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Behaviors
{
    public class BindPasswordBehavior : BridgeBehavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(BindPasswordBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Password_PropertyChanged));

        private static void Password_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
            }
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Pull();
        }

        protected override void PushCore()
        {
            AssociatedObject.Password = Password;
        }

        protected override void PullCore()
        {
            Password = AssociatedObject.Password;
        }
    }
}
