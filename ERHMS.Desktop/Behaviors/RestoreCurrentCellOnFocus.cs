using ERHMS.Desktop.Infrastructure;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class RestoreCurrentCellOnFocus : Behavior<DataGrid>
    {
        private bool restoring;
        private DataGridCellInfo oldCurrentCell;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CurrentCellChanged += AssociatedObject_CurrentCellChanged;
            AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_PreviewGotKeyboardFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.CurrentCellChanged -= AssociatedObject_CurrentCellChanged;
            AssociatedObject.PreviewGotKeyboardFocus -= AssociatedObject_PreviewGotKeyboardFocus;
        }

        private void AssociatedObject_CurrentCellChanged(object sender, EventArgs e)
        {
            SaveCurrentCell();
        }

        private void AssociatedObject_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!restoring
                && e.KeyboardDevice.IsKeyDown(Key.Tab)
                && !e.OldFocus.IsDescendantOf(AssociatedObject)
                && RestoreCurrentCell())
            {
                e.Handled = true;
            }
        }

        private bool SaveCurrentCell()
        {
            if (AssociatedObject.CurrentCell.IsValid)
            {
                oldCurrentCell = AssociatedObject.CurrentCell;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RestoreCurrentCell()
        {
            if (!oldCurrentCell.IsValid)
            {
                return false;
            }
            object item = oldCurrentCell.Item;
            if (!AssociatedObject.Items.Contains(item))
            {
                return false;
            }
            DataGridColumn column = oldCurrentCell.Column;
            if (column.DisplayIndex == -1)
            {
                if (AssociatedObject.Columns.Count == 0)
                {
                    return false;
                }
                column = AssociatedObject.ColumnFromDisplayIndex(0);
            }
            DataGridCell cell = GetCell(item, column);
            if (cell == null)
            {
                return false;
            }
            restoring = true;
            try
            {
                return cell.Focus();
            }
            finally
            {
                restoring = false;
            }
        }

        private DataGridCell GetCell(object item, DataGridColumn column)
        {
            FrameworkElement content = column.GetCellContent(item);
            if (content == null)
            {
                AssociatedObject.ScrollIntoView(item, column);
                content = column.GetCellContent(item);
            }
            return content?.Parent as DataGridCell;
        }
    }
}
