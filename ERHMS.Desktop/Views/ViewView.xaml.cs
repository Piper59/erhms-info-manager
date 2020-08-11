using ERHMS.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace ERHMS.Desktop.Views
{
    public partial class ViewView : UserControl
    {
        private static readonly TimeSpan SearchDelay = TimeSpan.FromSeconds(0.5);
        private static readonly IReadOnlyCollection<KeyValuePair<short?, string>> RecordStatuses = new KeyValuePair<short?, string>[]
        {
            new KeyValuePair<short?, string>(EpiInfo.Data.RecordStatus.Undeleted, "Undeleted"),
            new KeyValuePair<short?, string>(EpiInfo.Data.RecordStatus.Deleted, "Deleted"),
            new KeyValuePair<short?, string>(null, "All")
        };

        private static string GetRecordItemColumnHeader(string fieldName)
        {
            return fieldName.Replace("_", "__");
        }

        public new ViewViewModel DataContext
        {
            get { return (ViewViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        private DispatcherTimer searchTimer;

        public ViewView()
        {
            InitializeComponent();
            searchTimer = new DispatcherTimer
            {
                Interval = SearchDelay
            };
            searchTimer.Tick += (sender, e) => SetFilter();
            Search.TextChanged += Search_TextChanged;
            RecordStatus.ItemsSource = RecordStatuses;
            RecordStatus.DisplayMemberPath = "Value";
            RecordStatus.SelectedValuePath = "Key";
            RecordStatus.SelectedIndex = 0;
            RecordStatus.SelectionChanged += (sender, e) => SetFilter();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetRecordItemColumns();
            SetFilter();
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

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void SetFilter()
        {
            searchTimer.Stop();
            Predicate<ViewViewModel.RecordItem> searchFilter = recordItem => true;
            Predicate<ViewViewModel.RecordItem> recordStatusFilter = recordItem => true;
            if (Search.Text != "")
            {
                searchFilter = recordItem =>
                {
                    foreach (string fieldName in DataContext.FieldNames)
                    {
                        object value = recordItem.Record.Properties[fieldName];
                        if (value != null && value.ToString().IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
            if (RecordStatus.SelectedValue != null)
            {
                short recordStatus = (short)RecordStatus.SelectedValue;
                recordStatusFilter = recordItem => recordItem.Record.RecordStatus == recordStatus;
            }
            DataContext.RecordItems.TypedFilter = recordItem => searchFilter(recordItem) && recordStatusFilter(recordItem);
        }
    }
}
