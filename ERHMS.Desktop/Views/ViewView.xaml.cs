using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

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
        }
    }
}
