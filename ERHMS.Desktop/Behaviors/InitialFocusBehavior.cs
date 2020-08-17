using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class InitialFocusBehavior : Behavior<Panel>
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
            IInputElement element;
            if (ElementName == null)
            {
                element = AssociatedObject.Children.Count == 0 ? null : AssociatedObject.Children[0];
            }
            else
            {
                element = AssociatedObject.FindName(ElementName) as IInputElement;
            }
            if (element != null)
            {
                Keyboard.Focus(element);
            }
        }
    }
}
