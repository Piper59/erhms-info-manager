using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Metadata;
using System.Collections;
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
        public new RecordCollectionViewModel DataContext
        {
            get { return (RecordCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand CopyColumnCommand { get; }
        public ICommand CopyCellCommand { get; }

        public RecordCollectionView()
        {
            CopyColumnCommand = new SyncCommand<DataGridColumn>(CopyColumn);
            CopyCellCommand = new SyncCommand<DataGridCell>(CopyCell);
            InitializeComponent();
            DataContextChanged += RecordCollectionView_DataContextChanged;
        }

        private void RecordCollectionView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= DataContext_PropertyChanged;
            }
            if (e.NewValue != null)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged += DataContext_PropertyChanged;
                UpdateFields();
            }
        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecordCollectionViewModel.Fields))
            {
                UpdateFields();
            }
        }

        private void UpdateFields()
        {
            SetItemDataGridColumns();
            SetCopyColumnContextMenuItems();
        }

        private void SetItemDataGridColumns()
        {
            IReadOnlyList<FieldDataRow> fields = DataContext.Fields;
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

        private void SetCopyColumnContextMenuItems()
        {
            ItemCollection items = CopyColumnContextMenu.Items;
            items.Clear();
            if (ItemDataGrid.Columns.Count == 0)
            {
                return;
            }
            items.Add(new MenuItem
            {
                Command = CopyColumnCommand,
                Header = ResXResources.AccessText_AllColumns
            });
            items.Add(new Separator());
            foreach (DataGridColumn column in ItemDataGrid.Columns)
            {
                items.Add(new MenuItem
                {
                    Command = CopyColumnCommand,
                    CommandParameter = column,
                    Header = $"_{column.Header}"
                });
            }
        }

        private void Copy(IEnumerable<object> items, IReadOnlyCollection<DataGridColumn> columns)
        {
            IReadOnlyCollection<string> formats = new string[]
            {
                DataFormats.Text,
                DataFormats.UnicodeText,
                DataFormats.CommaSeparatedValue
            };
            IReadOnlyDictionary<string, StringBuilder> buildersByFormat =
                formats.ToDictionary(format => format, _ => new StringBuilder());
            int minColumnDisplayIndex = columns.Min(column => column.DisplayIndex);
            int maxColumnDisplayIndex = columns.Max(column => column.DisplayIndex);
            foreach (object item in items)
            {
                DataGridRowClipboardEventArgs e =
                    new DataGridRowClipboardEventArgs(item, minColumnDisplayIndex, maxColumnDisplayIndex, false);
                foreach (DataGridColumn column in columns)
                {
                    object content = column.OnCopyingCellClipboardContent(item);
                    e.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, column, content));
                }
                foreach (string format in formats)
                {
                    buildersByFormat[format].Append(e.FormatClipboardCellValues(format));
                }
            }
            DataObject data = new DataObject();
            foreach (string format in formats)
            {
                data.SetData(format, buildersByFormat[format].ToString());
            }
            Clipboard.SetDataObject(data);
        }

        public void CopyColumn(DataGridColumn column)
        {
            ICollection items = ItemDataGrid.SelectedIndex == -1 ? ItemDataGrid.Items : ItemDataGrid.SelectedItems;
            IReadOnlyCollection<DataGridColumn> columns;
            if (column == null)
            {
                columns = ItemDataGrid.Columns.Where(_column => _column.Visibility == Visibility.Visible)
                    .OrderBy(_column => _column.DisplayIndex)
                    .ToList();
            }
            else
            {
                columns = new DataGridColumn[]
                {
                    column
                };
            }
            Copy(items.Cast<object>(), columns);
        }

        public void CopyCell(DataGridCell cell)
        {
            IReadOnlyCollection<object> items = new object[]
            {
                cell.DataContext
            };
            IReadOnlyCollection<DataGridColumn> columns = new DataGridColumn[]
            {
                cell.Column
            };
            Copy(items, columns);
        }
    }
}
