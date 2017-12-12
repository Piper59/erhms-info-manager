using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class OpenContextMenuOnClickBehavior : Behavior<ButtonBase>
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
            OpenContextMenu();
            e.Handled = true;
        }

        private void OpenContextMenu()
        {
            if (AssociatedObject.ContextMenu == null)
            {
                return;
            }
            AssociatedObject.ContextMenu.DataContext = AssociatedObject.DataContext;
            AssociatedObject.ContextMenu.PlacementTarget = AssociatedObject;
            AssociatedObject.ContextMenu.IsOpen = true;
        }
    }
}
