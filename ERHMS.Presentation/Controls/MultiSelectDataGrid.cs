using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class MultiSelectDataGrid : DataGrid
    {
        public static readonly DependencyProperty MultiSelectedItemsProperty = DependencyProperty.Register(
            "MultiSelectedItems",
            typeof(IList),
            typeof(MultiSelectDataGrid),
            new FrameworkPropertyMetadata(null));
        public IList MultiSelectedItems
        {
            get { return (IList)GetValue(MultiSelectedItemsProperty); }
            set { SetValue(MultiSelectedItemsProperty, value); }
        }

        public MultiSelectDataGrid()
        {
            SelectionChanged += MultiSelectDataGrid_SelectionChanged;
        }

        private void MultiSelectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultiSelectedItems = SelectedItems;
        }
    }
}
