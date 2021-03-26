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

        public ICommand CopyDataCommand { get; }

        public RecordCollectionView()
        {
            InitializeComponent();
            Loaded += RecordCollectionView_Loaded;
            CopyDataCommand = new SyncCommand<DataGridColumn>(CopyData);
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
            SetCopyDataContextMenuItems();
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

        private void SetCopyDataContextMenuItems()
        {
            ItemCollection items = CopyDataContextMenu.Items;
            items.Clear();
            if (ItemDataGrid.Columns.Count == 0)
            {
                return;
            }
            items.Add(new MenuItem
            {
                Command = CopyDataCommand,
                Header = ResXResources.AccessText_All
            });
            items.Add(new Separator());
            foreach (DataGridColumn column in ItemDataGrid.Columns)
            {
                items.Add(new MenuItem
                {
                    Command = CopyDataCommand,
                    CommandParameter = column,
                    Header = $"_{column.Header}"
                });
            }
        }

        public void CopyData(DataGridColumn column)
        {
            IReadOnlyCollection<DataGridColumn> columns;
            int minColumnDisplayIndex;
            int maxColumnDisplayIndex;
            if (column == null)
            {
                columns = ItemDataGrid.Columns.OrderBy(_column => _column.DisplayIndex).ToList();
                minColumnDisplayIndex = columns.Min(_column => _column.DisplayIndex);
                maxColumnDisplayIndex = columns.Max(_column => _column.DisplayIndex);
            }
            else
            {
                columns = new DataGridColumn[]
                {
                    column
                };
                minColumnDisplayIndex = column.DisplayIndex;
                maxColumnDisplayIndex = column.DisplayIndex;
            }
            IReadOnlyCollection<string> formats = new string[]
            {
                DataFormats.Text,
                DataFormats.UnicodeText,
                DataFormats.CommaSeparatedValue
            };
            IReadOnlyDictionary<string, StringBuilder> builders =
                formats.ToDictionary(format => format, _ => new StringBuilder());
            foreach (RecordCollectionViewModel.ItemViewModel item in ViewModel.Items.SelectedItems)
            {
                DataGridRowClipboardEventArgs e =
                    new DataGridRowClipboardEventArgs(item, minColumnDisplayIndex, maxColumnDisplayIndex, false);
                foreach (DataGridColumn _column in columns)
                {
                    e.ClipboardRowContent.Add(
                        new DataGridClipboardCellContent(item, _column, _column.OnCopyingCellClipboardContent(item)));
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
    }
}
