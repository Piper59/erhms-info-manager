using ERHMS.Desktop.ViewModels;
using ERHMS.EpiInfo.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Views
{
    public partial class ViewView : UserControl
    {
        public ViewViewModel ViewModel => (ViewViewModel)DataContext;

        public ViewView()
        {
            InitializeComponent();
            Loaded += ViewView_Loaded;
        }

        private void ViewView_Loaded(object sender, RoutedEventArgs e)
        {
            SetRecordDataGridColumns();
        }

        private void SetRecordDataGridColumns()
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
    }
}
