using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class OpenContextMenuOnClick : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObject_MouseEvent;
            AssociatedObject.Click += AssociatedObject_MouseEvent;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
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
            if (AssociatedObject.ContextMenu == null)
            {
                return false;
            }
            else
            {
                AssociatedObject.ContextMenu.IsOpen = true;
                return true;
            }
        }
    }
}
