using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class BindPassword : BindProperty<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(BindPassword),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPropertyChanged));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
            Push();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
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
