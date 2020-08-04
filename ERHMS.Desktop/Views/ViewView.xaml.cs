using ERHMS.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Views
{
    public partial class ViewView : UserControl
    {
        public new ViewViewModel DataContext
        {
            get { return (ViewViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ViewView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetRecordColumns();
        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewViewModel.PropertyNames))
            {
                SetRecordColumns();
            }
        }

        private void SetRecordColumns()
        {
            Records.Columns.Clear();
            foreach (string propertyName in DataContext.PropertyNames)
            {
                Records.Columns.Add(new DataGridTextColumn
                {
                    Binding = new Binding(propertyName),
                    Header = propertyName
                });
            }
        }
    }
}
