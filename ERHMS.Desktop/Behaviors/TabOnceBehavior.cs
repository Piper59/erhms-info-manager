using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class TabOnceBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                switch (e.KeyboardDevice.Modifiers)
                {
                    case ModifierKeys.None:
                        MoveFocus(FocusNavigationDirection.Last, FocusNavigationDirection.Next);
                        e.Handled = true;
                        break;
                    case ModifierKeys.Shift:
                        MoveFocus(FocusNavigationDirection.First, FocusNavigationDirection.Previous);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void MoveFocus(FocusNavigationDirection direction1, FocusNavigationDirection direction2)
        {
            AssociatedObject.MoveFocus(new TraversalRequest(direction1));
            ((FrameworkElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(direction2));
        }
    }
}
