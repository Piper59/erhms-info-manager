using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class HomeView : UserControl
    {
        public HomeViewModel ViewModel => (HomeViewModel)DataContext;

        public HomeView()
        {
            InitializeComponent();
        }
    }
}
