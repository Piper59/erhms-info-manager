using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class BindPasswordBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(BindPasswordBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PasswordProperty_PropertyChanged));

        private static void PasswordProperty_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindPasswordBehavior)sender).OnPasswordChanged();
        }

        private bool updating;

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

        private void Update(Action action)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            action();
            updating = false;
        }

        private void OnPasswordChanged()
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
