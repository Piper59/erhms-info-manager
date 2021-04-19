using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class ProjectView : UserControl
    {
        public new ProjectViewModel DataContext
        {
            get { return (ProjectViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ProjectView()
        {
            InitializeComponent();
        }
    }
}
