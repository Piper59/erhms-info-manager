using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Desktop.Behaviors
{
    public class OpenContextMenuOnClick : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += AssociatedObject_MouseEvent;
            AssociatedObject.Click += AssociatedObject_MouseEvent;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= AssociatedObject_MouseEvent;
            AssociatedObject.Click -= AssociatedObject_MouseEvent;
        }

        private void AssociatedObject_MouseEvent(object sender, RoutedEventArgs e)
        {
            if (OpenContextMenu())
            {
                e.Handled = true;
            }
        }

        private bool OpenContextMenu()
        {
            ContextMenu contextMenu = AssociatedObject.ContextMenu;
            if (contextMenu == null)
            {
                return false;
            }
            else
            {
                contextMenu.PlacementTarget = AssociatedObject;
                contextMenu.IsOpen = true;
                return true;
            }
        }
    }
}
