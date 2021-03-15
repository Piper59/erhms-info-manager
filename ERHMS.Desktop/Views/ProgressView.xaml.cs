using ERHMS.Desktop.ViewModels;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class ProgressView : Window
    {
        public ProgressViewModel ViewModel => (ProgressViewModel)DataContext;

        public ProgressView()
        {
            InitializeComponent();
        }
    }
}
