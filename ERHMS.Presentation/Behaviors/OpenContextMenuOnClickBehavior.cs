using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class OpenContextMenuOnClickBehavior : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
        }

        private void AssociatedObject_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            OpenContextMenu();
            e.Handled = true;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            OpenContextMenu();
            e.Handled = true;
        }

        private void OpenContextMenu()
        {
            if (AssociatedObject.ContextMenu != null)
            {
                AssociatedObject.ContextMenu.DataContext = AssociatedObject.DataContext;
                AssociatedObject.ContextMenu.PlacementTarget = AssociatedObject;
                AssociatedObject.ContextMenu.IsOpen = true;
            }
        }
    }
}
