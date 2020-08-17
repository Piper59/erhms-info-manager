using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Desktop.Behaviors
{
    public class DropDownButtonBehavior : Behavior<ButtonBase>
    {
        private ContextMenu ContextMenu => AssociatedObject.ContextMenu;

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
            ContextMenu.PlacementTarget = AssociatedObject;
            ContextMenu.Placement = PlacementMode.Right;
            ContextMenu.IsOpen = true;
            e.Handled = true;
        }
    }
}
