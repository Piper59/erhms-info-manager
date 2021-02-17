using ERHMS.Desktop.Infrastructure;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class MoveFocusInternallyOnControlArrow : Behavior<DataGrid>
    {
        private static bool IsArrowKey(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                    return true;
                default:
                    return false;
            }
        }

        private static FocusNavigationDirection GetDirection(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return FocusNavigationDirection.Left;
                case Key.Up:
                    return FocusNavigationDirection.Up;
                case Key.Right:
                    return FocusNavigationDirection.Right;
                case Key.Down:
                    return FocusNavigationDirection.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

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
            if (IsArrowKey(e.Key)
                && e.KeyboardDevice.Modifiers == ModifierKeys.Control
                && Keyboard.FocusedElement.MoveFocus(GetDirection(e.Key)))
            {
                e.Handled = true;
            }
        }
    }
}
