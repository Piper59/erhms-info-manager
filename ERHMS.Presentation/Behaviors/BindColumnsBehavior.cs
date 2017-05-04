using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class BindColumnsBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(ICollection<DataGridColumn>),
            typeof(BindColumnsBehavior),
            new FrameworkPropertyMetadata(ColumnsProperty_PropertyChanged));

        private static void ColumnsProperty_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindColumnsBehavior)sender).OnColumnsChanged();
        }

        public ICollection<DataGridColumn> Columns
        {
            get { return (ICollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        private void OnColumnsChanged()
        {
            AssociatedObject.Columns.Clear();
            if (Columns != null)
            {
                foreach (DataGridColumn column in Columns)
                {
                    AssociatedObject.Columns.Add(column);
                }
            }
        }
    }
}
