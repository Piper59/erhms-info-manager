using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class TabOnceBehavior : Behavior<DataGrid>
    {
        private bool settingFocus;
        private DataGridCellInfo focusedCell;

        protected override void OnAttached()
        {
            AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!settingFocus && IsDescendantCell(e.NewFocus, out DataGridCell newCell))
            {
                if (!e.KeyboardDevice.IsKeyDown(Key.Tab) || IsDescendantCell(e.OldFocus, out DataGridCell oldCell))
                {
                    focusedCell = AssociatedObject.CurrentCell;
                }
                else if (AssociatedObject.Columns.Count > 0 && AssociatedObject.Items.Count > 0)
                {
                    IList items = AssociatedObject.SelectedItems.Count > 0 ? AssociatedObject.SelectedItems : AssociatedObject.Items;
                    object item = items[0];
                    DataGridColumn column = AssociatedObject.ColumnFromDisplayIndex(0);
                    if (focusedCell != null && items.Contains(focusedCell.Item))
                    {
                        item = focusedCell.Item;
                        if (focusedCell.Column.DisplayIndex != -1)
                        {
                            column = focusedCell.Column;
                        }
                    }
                    SetFocus(item, column);
                }
            }
        }

        private bool IsDescendantCell(IInputElement element, out DataGridCell cell)
        {
            cell = element as DataGridCell;
            return cell != null && AssociatedObject.IsAncestorOf(cell);
        }

        private void SetFocus(object item, DataGridColumn column)
        {
            settingFocus = true;
            try
            {
                DataGridCellInfo cell = new DataGridCellInfo(item, column);
                AssociatedObject.CurrentCell = cell;
                switch (AssociatedObject.SelectionMode)
                {
                    case DataGridSelectionMode.Single:
                        AssociatedObject.SelectedItem = item;
                        break;
                    case DataGridSelectionMode.Extended:
                        if (!AssociatedObject.SelectedItems.Contains(item))
                        {
                            AssociatedObject.SelectedItems.Add(item);
                        }
                        break;
                }
                focusedCell = cell;
            }
            catch { }
            finally
            {
                settingFocus = false;
            }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                switch (e.KeyboardDevice.Modifiers)
                {
                    case ModifierKeys.None:
                        MoveFocus(true);
                        e.Handled = true;
                        break;
                    case ModifierKeys.Shift:
                        MoveFocus(false);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void MoveFocus(bool forward)
        {
            bool intercepted = false;

            void handler(object sender, KeyboardFocusChangedEventArgs e)
            {
                intercepted = true;
                MoveFocusOuter(e.NewFocus, forward);
                e.Handled = true;
            }

            AssociatedObject.PreviewGotKeyboardFocus += handler;
            try
            {
                MoveFocusInner(forward);
                if (!intercepted)
                {
                    MoveFocusOuter(Keyboard.FocusedElement, forward);
                }
            }
            catch { }
            finally
            {
                AssociatedObject.PreviewGotKeyboardFocus -= handler;
            }
        }

        private void MoveFocusInner(bool forward)
        {
            FocusNavigationDirection direction = forward ? FocusNavigationDirection.Last : FocusNavigationDirection.First;
            AssociatedObject.MoveFocus(new TraversalRequest(direction));
        }

        private void MoveFocusOuter(IInputElement element, bool forward)
        {
            FocusNavigationDirection direction = forward ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous;
            ((FrameworkElement)element).MoveFocus(new TraversalRequest(direction));
        }
    }
}
