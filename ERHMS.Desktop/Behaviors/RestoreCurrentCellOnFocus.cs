﻿using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ERHMS.Desktop.Behaviors
{
    public class RestoreCurrentCellOnFocus : Behavior<DataGrid>
    {
        private bool restoring;
        private DataGridCellInfo oldCurrentCell;

        protected override void OnAttached()
        {
            AssociatedObject.CurrentCellChanged += AssociatedObject_CurrentCellChanged;
            AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_PreviewGotKeyboardFocus;
        }

        protected override void OnDetaching()
        {
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
                && e.OldFocus is Visual visual
                && !visual.IsDescendantOf(AssociatedObject)
                && RestoreCurrentCell())
            {
                e.Handled = true;
            }
        }

        private bool SaveCurrentCell()
        {
            if (!AssociatedObject.CurrentCell.IsValid)
            {
                return false;
            }
            oldCurrentCell = AssociatedObject.CurrentCell;
            return true;
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