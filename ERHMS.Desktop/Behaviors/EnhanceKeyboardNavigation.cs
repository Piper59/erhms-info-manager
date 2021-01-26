using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class EnhanceKeyboardNavigation : Behavior<DataGrid>
    {
        private static IEnumerable<MouseButtonState> MouseButtonStates
        {
            get
            {
                yield return Mouse.LeftButton;
                yield return Mouse.RightButton;
                yield return Mouse.MiddleButton;
                yield return Mouse.XButton1;
                yield return Mouse.XButton2;
            }
        }

        private static FocusNavigationDirection GetInternalDirection(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return FocusNavigationDirection.Left;
                case Key.Up:
                    return FocusNavigationDirection.Up;
                case Key.Right:
                    return FocusNavigationDirection.Right;
                case Key.Down:
                    return FocusNavigationDirection.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

        private static bool TryGetExternalDirection(ModifierKeys modifiers, out FocusNavigationDirection direction)
        {
            switch (modifiers)
            {
                case ModifierKeys.None:
                    direction = FocusNavigationDirection.Next;
                    return true;
                case ModifierKeys.Shift:
                    direction = FocusNavigationDirection.Previous;
                    return true;
                default:
                    direction = default;
                    return false;
            }
        }

        private static FocusNavigationDirection GetTerminalDirection(FocusNavigationDirection direction)
        {
            switch (direction)
            {
                case FocusNavigationDirection.Next:
                    return FocusNavigationDirection.Last;
                case FocusNavigationDirection.Previous:
                    return FocusNavigationDirection.First;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
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
            if (IsDescendantCell(e.NewFocus))
            {
                if (MouseButtonStates.Contains(MouseButtonState.Pressed) || IsDescendantCell(e.OldFocus))
                {
                    SaveFocus();
                }
                else
                {
                    RestoreFocus();
                }
            }
        }

        private bool IsDescendantCell(IInputElement element)
        {
            return element is DataGridCell cell && AssociatedObject.IsAncestorOf(cell);
        }

        private void SaveFocus()
        {
            focusedCell = AssociatedObject.CurrentCell;
        }

        private void RestoreFocus()
        {
            if (AssociatedObject.Items.Count > 0 && AssociatedObject.Columns.Count > 0)
            {
                object item = AssociatedObject.Items[0];
                DataGridColumn column = AssociatedObject.ColumnFromDisplayIndex(0);
                if (focusedCell != null)
                {
                    if (AssociatedObject.Items.Contains(focusedCell.Item))
                    {
                        item = focusedCell.Item;
                    }
                    if (focusedCell.Column.DisplayIndex != -1)
                    {
                        column = focusedCell.Column;
                    }
                }
                AssociatedObject.CurrentCell = new DataGridCellInfo(item, column);
            }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    if (MoveFocusExternal(e.KeyboardDevice.Modifiers))
                    {
                        e.Handled = true;
                    }
                    break;
                case Key.Space:
                    if (ToggleSelection())
                    {
                        e.Handled = true;
                    }
                    break;
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                    if (MoveFocusInternal(e.Key, e.KeyboardDevice.Modifiers))
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        private bool MoveFocus(IInputElement element, FocusNavigationDirection direction)
        {
            if (element is FrameworkElement frameworkElement)
            {
                return frameworkElement.MoveFocus(new TraversalRequest(direction));
            }
            else
            {
                return false;
            }
        }

        private bool MoveFocus(FocusNavigationDirection direction)
        {
            return MoveFocus(Keyboard.FocusedElement, direction);
        }

        private bool MoveFocusInternal(Key key, ModifierKeys modifiers)
        {
            if (modifiers == ModifierKeys.Control)
            {
                return MoveFocus(GetInternalDirection(key));
            }
            else
            {
                return false;
            }
        }

        private bool MoveFocusExternal(ModifierKeys modifiers)
        {
            if (TryGetExternalDirection(modifiers, out FocusNavigationDirection direction))
            {
                bool intercepted = false;
                bool result = false;

                void AssociatedObject_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
                {
                    intercepted = true;
                    result = MoveFocus(e.NewFocus, direction);
                    e.Handled = true;
                }

                AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_PreviewGotKeyboardFocus;
                try
                {
                    MoveFocus(GetTerminalDirection(direction));
                    if (!intercepted)
                    {
                        result = MoveFocus(direction);
                    }
                }
                finally
                {
                    AssociatedObject.PreviewGotKeyboardFocus -= AssociatedObject_PreviewGotKeyboardFocus;
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        private bool ToggleSelection()
        {
            if (AssociatedObject.CurrentItem == null)
            {
                return false;
            }
            else
            {
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
}
