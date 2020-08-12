using ERHMS.Desktop.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Views
{
    public partial class ViewView : UserControl
    {
        private static string GetRecordItemColumnHeader(string fieldName)
        {
            return fieldName.Replace("_", "__");
        }

        public new ViewViewModel DataContext
        {
            get { return (ViewViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ViewView()
        {
            InitializeComponent();
            Loaded += (sender, e) => SetRecordItemColumns();
        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewViewModel.FieldNames))
            {
                SetRecordItemColumns();
            }
        }

        private void SetRecordItemColumns()
        {
            IDictionary<string, DataGridColumn> columns = RecordItems.Columns.ToDictionary(column => (string)column.Header);
            for (int fieldNameIndex = 0; fieldNameIndex < DataContext.FieldNames.Count; fieldNameIndex++)
            {
                string fieldName = DataContext.FieldNames[fieldNameIndex];
                if (columns.TryGetValue(GetRecordItemColumnHeader(fieldName), out DataGridColumn column))
                {
                    int columnIndex = RecordItems.Columns.IndexOf(column);
                    if (columnIndex != fieldNameIndex)
                    {
                        RecordItems.Columns.Move(columnIndex, fieldNameIndex);
                    }
                }
                else
                {
                    RecordItems.Columns.Insert(fieldNameIndex, GetRecordItemColumn(fieldName));
                }
            }
            while (RecordItems.Columns.Count > DataContext.FieldNames.Count)
            {
                RecordItems.Columns.RemoveAt(RecordItems.Columns.Count - 1);
            }
        }

        private DataGridColumn GetRecordItemColumn(string fieldName)
        {
            return new DataGridTextColumn
            {
                Binding = new Binding($"Record.{fieldName}"),
                ElementStyle = (Style)FindResource("Field"),
                Header = GetRecordItemColumnHeader(fieldName)
            };
        }
    }
}
