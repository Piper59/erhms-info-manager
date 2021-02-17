using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class ToggleSelectionOnSpace : Behavior<DataGrid>
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
            if (e.Key == Key.Space && ToggleSelection())
            {
                e.Handled = true;
            }
        }

        private bool ToggleSelection()
        {
            if (AssociatedObject.CurrentItem == null)
            {
                return false;
            }
            switch (AssociatedObject.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    if (Equals(AssociatedObject.SelectedItem, AssociatedObject.CurrentItem))
                    {
                        AssociatedObject.SelectedItem = null;
                    }
                    else
                    {
                        AssociatedObject.SelectedItem = AssociatedObject.CurrentItem;
                    }
                    return true;
                case DataGridSelectionMode.Extended:
                    int index = AssociatedObject.SelectedItems.IndexOf(AssociatedObject.CurrentItem);
                    if (index == -1)
                    {
                        AssociatedObject.SelectedItems.Add(AssociatedObject.CurrentItem);
                    }
                    else
                    {
                        AssociatedObject.SelectedItems.RemoveAt(index);
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}
