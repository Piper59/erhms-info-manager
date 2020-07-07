using ERHMS.Desktop.ViewModels;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public MainView(MainViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
