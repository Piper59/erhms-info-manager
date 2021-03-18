using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class RecordCollectionView : UserControl
    {
        private const double InitialMaxColumnWidth = 320.0;

        public RecordCollectionViewModel ViewModel => (RecordCollectionViewModel)DataContext;

        public RecordCollectionView()
        {
            InitializeComponent();
            Loaded += RecordCollectionView_Loaded;
            RecordDataGrid.LayoutUpdated += RecordDataGrid_LayoutUpdated;
        }

        private void RecordCollectionView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FieldDataRow field in ViewModel.Fields)
            {
                DataGridColumn column = new DataGridTextColumn
                {
                    Binding = new Binding($"Value.{field.Name}"),
                    Header = field.Name.Replace("_", "__")
                };
                if (field.FieldType.IsNumeric())
                {
                    column.CellStyle = (Style)FindResource("NumericDataGridCell");
                }
                RecordDataGrid.Columns.Add(column);
            }
        }

        private void RecordDataGrid_LayoutUpdated(object sender, System.EventArgs e)
        {
            if (RecordDataGrid.Columns.Count > 0)
            {
                RecordDataGrid.LayoutUpdated -= RecordDataGrid_LayoutUpdated;
            }
            foreach (DataGridColumn column in RecordDataGrid.Columns)
            {
                if (column.ActualWidth > InitialMaxColumnWidth)
                {
                    column.Width = new DataGridLength(InitialMaxColumnWidth);
                }
            }
        }
    }
}
