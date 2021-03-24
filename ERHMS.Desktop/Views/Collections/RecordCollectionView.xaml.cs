using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class RecordCollectionView : UserControl
    {
        public RecordCollectionViewModel ViewModel => (RecordCollectionViewModel)DataContext;

        public RecordCollectionView()
        {
            InitializeComponent();
            Loaded += RecordCollectionView_Loaded;
        }

        private void RecordCollectionView_Loaded(object sender, RoutedEventArgs e)
        {
            SetRecordDataGridColumns();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecordCollectionViewModel.Fields))
            {
                SetRecordDataGridColumns();
            }
        }

        private void SetRecordDataGridColumns()
        {
            IReadOnlyList<FieldDataRow> fields = ViewModel.Fields.ToList();
            ObservableCollection<DataGridColumn> columns = RecordDataGrid.Columns;
            IDictionary<string, DataGridColumn> columnsByHeader = columns.ToDictionary(column => (string)column.Header);
            for (int fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
            {
                FieldDataRow field = fields[fieldIndex];
                string header = field.Name.Replace("_", "__");
                if (columnsByHeader.TryGetValue(header, out DataGridColumn column))
                {
                    int columnIndex = columns.IndexOf(column);
                    if (columnIndex != fieldIndex)
                    {
                        columns.Move(columnIndex, fieldIndex);
                    }
                }
                else
                {
                    column = new DataGridTextColumn
                    {
                        Binding = new Binding($"Value.{field.Name}"),
                        ElementStyle = (Style)FindResource("CellTextBlock"),
                        Header = header
                    };
                    if (field.FieldType.IsNumeric())
                    {
                        column.CellStyle = (Style)FindResource("NumericDataGridCell");
                    }
                    columns.Insert(fieldIndex, column);
                }
            }
            while (columns.Count > fields.Count)
            {
                columns.RemoveAt(columns.Count - 1);
            }
        }
    }
}
