using System.Windows.Controls;
using ERHMS.Presentation.ViewModel;
using System.Diagnostics;
using System.Windows.Navigation;
using System.IO;
using System.Reflection;

namespace ERHMS.Presentation.View
{
    /// <summary>
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void RelativeHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string workingDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string path = System.IO.Path.Combine(workingDirectory, e.Uri.ToString());
            Process.Start(path);
            e.Handled = true;
        }
    }
}
