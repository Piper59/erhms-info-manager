using ERHMS.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public MainView(MainViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainViewModel.Content):
                    ContentViewer.ScrollToTop();
                    break;
            }
        }
    }
}
