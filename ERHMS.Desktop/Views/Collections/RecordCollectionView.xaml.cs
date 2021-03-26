using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class RecordCollectionView : UserControl
    {
        public RecordCollectionViewModel ViewModel => (RecordCollectionViewModel)DataContext;

        public ICommand CopyColumnDataCommand { get; }
        public ICommand CopyCellDataCommand { get; }

        public RecordCollectionView()
        {
            InitializeComponent();
            Loaded += RecordCollectionView_Loaded;
            CopyColumnDataCommand = new SyncCommand<DataGridColumn>(CopyColumnData);
            CopyCellDataCommand = new SyncCommand<DataGridCell>(CopyCellData);
        }

        private void RecordCollectionView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFields();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecordCollectionViewModel.Fields))
            {
                UpdateFields();
            }
        }

        private void UpdateFields()
        {
            SetItemDataGridColumns();
            SetCopyColumnDataContextMenuItems();
        }

        private void SetItemDataGridColumns()
        {
            IReadOnlyList<FieldDataRow> fields = ViewModel.Fields.ToList();
            ObservableCollection<DataGridColumn> columns = ItemDataGrid.Columns;
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
                        column.CellStyle = (Style)FindResource("CopyableNumericDataGridCell");
                    }
                    columns.Insert(fieldIndex, column);
                }
            }
            while (columns.Count > fields.Count)
            {
                columns.RemoveAt(columns.Count - 1);
            }
        }

        private void SetCopyColumnDataContextMenuItems()
        {
            ItemCollection items = CopyColumnDataContextMenu.Items;
            items.Clear();
            if (ItemDataGrid.Columns.Count == 0)
            {
                return;
            }
            items.Add(new MenuItem
            {
                Command = CopyColumnDataCommand,
                Header = ResXResources.AccessText_All
            });
            items.Add(new Separator());
            foreach (DataGridColumn column in ItemDataGrid.Columns)
            {
                items.Add(new MenuItem
                {
                    Command = CopyColumnDataCommand,
                    CommandParameter = column,
                    Header = $"_{column.Header}"
                });
            }
        }

        private void CopyData(IReadOnlyCollection<DataGridColumn> columns, IEnumerable<object> items)
        {
            int minColumnDisplayIndex = columns.Min(column => column.DisplayIndex);
            int maxColumnDisplayIndex = columns.Max(column => column.DisplayIndex);
            IReadOnlyCollection<string> formats = new string[]
            {
                DataFormats.Text,
                DataFormats.UnicodeText,
                DataFormats.CommaSeparatedValue
            };
            IReadOnlyDictionary<string, StringBuilder> builders =
                formats.ToDictionary(format => format, _ => new StringBuilder());
            foreach (object item in items)
            {
                DataGridRowClipboardEventArgs e =
                    new DataGridRowClipboardEventArgs(item, minColumnDisplayIndex, maxColumnDisplayIndex, false);
                foreach (DataGridColumn column in columns)
                {
                    e.ClipboardRowContent.Add(
                        new DataGridClipboardCellContent(item, column, column.OnCopyingCellClipboardContent(item)));
                }
                foreach (string format in formats)
                {
                    builders[format].Append(e.FormatClipboardCellValues(format));
                }
            }
            DataObject data = new DataObject();
            foreach (string format in formats)
            {
                data.SetData(format, builders[format].ToString());
            }
            Clipboard.SetDataObject(data);
        }

        public void CopyColumnData(DataGridColumn column)
        {
            IReadOnlyCollection<DataGridColumn> columns;
            if (column == null)
            {
                columns = ItemDataGrid.Columns.OrderBy(_column => _column.DisplayIndex).ToList();
            }
            else
            {
                columns = new DataGridColumn[]
                {
                    column
                };
            }
            CopyData(columns, ViewModel.Items.SelectedItems);
        }

        public void CopyCellData(DataGridCell cell)
        {
            IReadOnlyCollection<DataGridColumn> columns = new DataGridColumn[]
            {
                cell.Column
            };
            IEnumerable<object> items = new object[]
            {
                cell.DataContext
            };
            CopyData(columns, items);
        }
    }
}
