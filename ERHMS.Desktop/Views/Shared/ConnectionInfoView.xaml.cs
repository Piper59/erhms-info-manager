using ERHMS.Desktop.ViewModels.Shared;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Shared
{
    public partial class ConnectionInfoView : UserControl
    {
        public new ConnectionInfoViewModel DataContext
        {
            get { return (ConnectionInfoViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ConnectionInfoView()
        {
            InitializeComponent();
        }
    }
}
