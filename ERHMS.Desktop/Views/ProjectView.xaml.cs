using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class ProjectView : UserControl
    {
        public ProjectViewModel ViewModel => (ProjectViewModel)DataContext;

        public ProjectView()
        {
            InitializeComponent();
        }
    }
}
