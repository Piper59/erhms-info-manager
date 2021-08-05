using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class OpenContextMenuOnClick : Behavior<Button>
    {
        private bool isOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (OpenContextMenu())
            {
                e.Handled = true;
            }
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            AssociatedObject.ContextMenu.Closed -= ContextMenu_Closed;
            isOpen = false;
        }

        private bool OpenContextMenu()
        {
            ContextMenu contextMenu = AssociatedObject.ContextMenu;
            if (contextMenu == null || isOpen)
            {
                return false;
            }
            else
            {
                contextMenu.Closed += ContextMenu_Closed;
                contextMenu.PlacementTarget = AssociatedObject;
                contextMenu.Placement = ContextMenuService.GetPlacement(AssociatedObject);
                contextMenu.IsOpen = true;
                isOpen = true;
                return true;
            }
        }
    }
}
