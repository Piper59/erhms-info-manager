using ERHMS.Common.Linq;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Naming;
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

        public ICommand CopyFieldCommand { get; }
        public ICommand CopyValueCommand { get; }

        public RecordCollectionView()
        {
            CopyFieldCommand = new SyncCommand<DataGridColumn>(CopyField);
            CopyValueCommand = new SyncCommand<DataGridCell>(CopyValue);
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
            SetCopyFieldContextMenuItems();
        }

        private void SetItemDataGridColumns()
        {
            ObservableCollection<DataGridColumn> columns = ItemDataGrid.Columns;
            IDictionary<string, DataGridColumn> columnsByHeader =
                columns.ToDictionary(column => (string)column.Header, NameComparer.Default);
            foreach (Iterator<FieldDataRow> field in DataContext.Fields.Iterate())
            {
                string header = field.Value.Name.Replace("_", "__");
                if (columnsByHeader.TryGetValue(header, out DataGridColumn column))
                {
                    int columnIndex = columns.IndexOf(column);
                    if (columnIndex != field.Index)
                    {
                        columns.Move(columnIndex, field.Index);
                    }
                }
                else
                {
                    column = new DataGridTextColumn
                    {
                        Binding = new Binding($"Value.{field.Value.Name}"),
                        ElementStyle = (Style)FindResource("CellTextBlock"),
                        Header = header
                    };
                    if (field.Value.FieldType.IsNumeric())
                    {
                        column.CellStyle = (Style)FindResource("CopyableNumericDataGridCell");
                    }
                    columns.Insert(field.Index, column);
                }
            }
            int fieldCount = DataContext.Fields.Count();
            while (columns.Count > fieldCount)
            {
                columns.RemoveAt(columns.Count - 1);
            }
        }

        private void SetCopyFieldContextMenuItems()
        {
            ItemCollection items = CopyFieldContextMenu.Items;
            items.Clear();
            items.Add(new MenuItem
            {
                Command = CopyFieldCommand,
                Header = ResXResources.AccessText_AllFields
            });
            items.Add(new Separator());
            foreach (DataGridColumn column in ItemDataGrid.Columns)
            {
                items.Add(new MenuItem
                {
                    Command = CopyFieldCommand,
                    CommandParameter = column,
                    Header = $"_{column.Header}"
                });
            }
        }

        private void Copy(IEnumerable items, int startColumnDisplayIndex, int endColumnDisplayIndex)
        {
            IEnumerable<string> formats = new string[]
            {
                DataFormats.Text,
                DataFormats.UnicodeText,
                DataFormats.CommaSeparatedValue
            };
            IDictionary<string, StringBuilder> buildersByFormat =
                formats.ToDictionary(format => format, _ => new StringBuilder());
            foreach (object item in items)
            {
                DataGridRowClipboardEventArgs e =
                    new DataGridRowClipboardEventArgs(item, startColumnDisplayIndex, endColumnDisplayIndex, false);
                foreach (DataGridColumn column in ItemDataGrid.Columns)
                {
                    if (column.DisplayIndex < startColumnDisplayIndex || column.DisplayIndex > endColumnDisplayIndex)
                    {
                        continue;
                    }
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

        public void CopyField(DataGridColumn column)
        {
            IEnumerable items = ItemDataGrid.SelectedIndex == -1 ? ItemDataGrid.Items : ItemDataGrid.SelectedItems;
            if (column == null)
            {
                Copy(items, 0, ItemDataGrid.Columns.Count - 1);
            }
            else
            {
                Copy(items, column.DisplayIndex, column.DisplayIndex);
            }
        }

        public void CopyValue(DataGridCell cell)
        {
            IEnumerable items = IEnumerableExtensions.Yield(cell.DataContext);
            Copy(items, cell.Column.DisplayIndex, cell.Column.DisplayIndex);
        }
    }
}
