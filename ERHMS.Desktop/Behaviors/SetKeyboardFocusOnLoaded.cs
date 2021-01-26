using Microsoft.Xaml.Behaviors;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class SetKeyboardFocusOnLoaded : Behavior<Panel>
    {
        public string ElementName { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocus();
        }

        private bool SetFocus()
        {
            IInputElement element = ElementName == null
                ? AssociatedObject.Children.Cast<IInputElement>().FirstOrDefault()
                : AssociatedObject.FindName(ElementName) as IInputElement;
            if (element == null)
            {
                return false;
            }
            else
            {
                Keyboard.Focus(element);
                return true;
            }
        }
    }
}
