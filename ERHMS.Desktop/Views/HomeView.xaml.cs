using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class HomeView : UserControl
    {
        public new HomeViewModel DataContext
        {
            get { return (HomeViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public HomeView()
        {
            InitializeComponent();
        }
    }
}
