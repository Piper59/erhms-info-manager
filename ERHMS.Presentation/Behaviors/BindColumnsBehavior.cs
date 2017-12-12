using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Behaviors
{
    public class BindColumnsBehavior : BridgeBehavior<DataGrid>
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(ICollection<DataGridColumn>),
            typeof(BindColumnsBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Columns_PropertyChanged));

        private static void Columns_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindColumnsBehavior)sender).Push();
        }

        public ICollection<DataGridColumn> Columns
        {
            get { return (ICollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Columns.CollectionChanged += AssociatedObject_Columns_CollectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Columns.CollectionChanged -= AssociatedObject_Columns_CollectionChanged;
        }

        private void AssociatedObject_Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Pull();
        }

        protected override void PushCore()
        {
            AssociatedObject.Columns.Clear();
            if (Columns == null)
            {
                return;
            }
            foreach (DataGridColumn column in Columns)
            {
                AssociatedObject.Columns.Add(column);
            }
        }

        protected override void PullCore()
        {
            Columns = AssociatedObject.Columns.ToList();
        }
    }
}
