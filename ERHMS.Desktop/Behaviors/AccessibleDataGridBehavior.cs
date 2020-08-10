using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class AccessibleDataGridBehavior : Behavior<DataGrid>
    {
        private static bool IsAnyMouseButtonPressed()
        {
            return
                Mouse.LeftButton == MouseButtonState.Pressed
                || Mouse.MiddleButton == MouseButtonState.Pressed
                || Mouse.RightButton == MouseButtonState.Pressed
                || Mouse.XButton1 == MouseButtonState.Pressed
                || Mouse.XButton2 == MouseButtonState.Pressed;
        }

        private static FocusNavigationDirection GetDirection(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    return FocusNavigationDirection.Up;
                case Key.Down:
                    return FocusNavigationDirection.Down;
                case Key.Left:
                    return FocusNavigationDirection.Left;
                case Key.Right:
                    return FocusNavigationDirection.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

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
            if (IsDescendantCell(e.NewFocus, out DataGridCell newCell))
            {
                if (IsAnyMouseButtonPressed() || IsDescendantCell(e.OldFocus, out DataGridCell oldCell))
                {
                    focusedCell = AssociatedObject.CurrentCell;
                }
                else if (AssociatedObject.Columns.Count > 0 && AssociatedObject.Items.Count > 0)
                {
                    object item = AssociatedObject.Items[0];
                    DataGridColumn column = AssociatedObject.ColumnFromDisplayIndex(0);
                    if (focusedCell != null && AssociatedObject.Items.Contains(focusedCell.Item))
                    {
                        item = focusedCell.Item;
                        if (focusedCell.Column.DisplayIndex != -1)
                        {
                            column = focusedCell.Column;
                        }
                    }
                    AssociatedObject.CurrentCell = new DataGridCellInfo(item, column);
                }
            }
        }

        private bool IsDescendantCell(IInputElement element, out DataGridCell cell)
        {
            cell = element as DataGridCell;
            return cell != null && AssociatedObject.IsAncestorOf(cell);
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (AssociatedObject.CurrentCell.IsValid)
                    {
                        ToggleSelection(AssociatedObject.CurrentCell.Item);
                        e.Handled = true;
                    }
                    break;
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        ((FrameworkElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(GetDirection(e.Key)));
                        e.Handled = true;
                    }
                    break;
                case Key.Tab:
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
                    break;
            }
        }

        private void ToggleSelection(object item)
        {
            switch (AssociatedObject.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    if (AssociatedObject.SelectedItem == item)
                    {
                        AssociatedObject.SelectedItem = null;
                    }
                    else
                    {
                        AssociatedObject.SelectedItem = item;
                    }
                    break;
                case DataGridSelectionMode.Extended:
                    int index = AssociatedObject.SelectedItems.IndexOf(item);
                    if (index == -1)
                    {
                        AssociatedObject.SelectedItems.Add(item);
                    }
                    else
                    {
                        AssociatedObject.SelectedItems.RemoveAt(index);
                    }
                    break;
            }
        }

        private void MoveFocus(bool forward)
        {
            bool intercepted = false;

            void AssociatedObject_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
            {
                intercepted = true;
                MoveFocusOuter(e.NewFocus, forward);
                e.Handled = true;
            }

            AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_PreviewGotKeyboardFocus;
            MoveFocusInner(forward);
            if (!intercepted)
            {
                MoveFocusOuter(Keyboard.FocusedElement, forward);
            }
            AssociatedObject.PreviewGotKeyboardFocus -= AssociatedObject_PreviewGotKeyboardFocus;
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
