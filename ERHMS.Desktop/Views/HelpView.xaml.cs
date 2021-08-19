using ERHMS.Desktop.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace ERHMS.Desktop.Views
{
    public partial class HelpView : Window
    {
        public new HelpViewModel DataContext
        {
            get { return (HelpViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public HelpView()
        {
            InitializeComponent();
        }

        private void FlowDocumentScrollViewer_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString())?.Dispose();
        }
    }
}
