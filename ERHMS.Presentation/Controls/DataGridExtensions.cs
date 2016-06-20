using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Controls
{
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty BoundColumnsProperty = DependencyProperty.RegisterAttached(
            "BoundColumns",
            typeof(ICollection<DataGridColumn>),
            typeof(DataGridExtensions),
            new PropertyMetadata(BoundColumns_PropertyChanged));
        public static ICollection<DataGridColumn> GetBoundColumns(DataGrid target)
        {
            return (ICollection<DataGridColumn>)target.GetValue(BoundColumnsProperty);
        }
        public static void SetBoundColumns(DataGrid target, ICollection<DataGridColumn> value)
        {
            target.SetValue(BoundColumnsProperty, value);
        }

        private static void BoundColumns_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid target = (DataGrid)sender;
            ICollection<DataGridColumn> columns = (ICollection<DataGridColumn>)e.NewValue;
            target.Columns.Clear();
            if (columns != null)
            {
                foreach (DataGridColumn column in columns)
                {
                    target.Columns.Add(column);
                }
            }
        }

        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached(
            "DoubleClickCommand",
            typeof(ICommand),
            typeof(DataGridExtensions),
            new PropertyMetadata(DoubleClickCommand_PropertyChanged));
        public static ICommand GetDoubleClickCommand(DataGrid target)
        {
            return (ICommand)target.GetValue(DoubleClickCommandProperty);
        }
        public static void SetDoubleClickCommand(DataGrid target, ICommand value)
        {
            target.SetValue(DoubleClickCommandProperty, value);
        }

        private static void DoubleClickCommand_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid target = (DataGrid)sender;
            target.MouseDoubleClick -= DoubleClickCommand_Target_MouseDoubleClick;
            if (e.NewValue != null)
            {
                target.MouseDoubleClick += DoubleClickCommand_Target_MouseDoubleClick;
            }
        }

        private static void DoubleClickCommand_Target_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid target = (DataGrid)sender;
            ICommand command = GetDoubleClickCommand(target);
            if (command.CanExecute(target))
            {
                command.Execute(target);
            }
        }
    }
}
