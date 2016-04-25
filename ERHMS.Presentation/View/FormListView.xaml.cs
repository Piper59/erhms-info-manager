using ERHMS.Presentation.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace ERHMS.Presentation.View
{
    public partial class FormListView
    {
        public FormListView()
        {
            DataContext = new FormListViewModel();
            InitializeComponent();
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            btnPublish.ContextMenu.DataContext = btnPublish.DataContext;
            btnPublish.ContextMenu.IsOpen = true;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            btnImport.ContextMenu.DataContext = btnImport.DataContext;
            btnImport.ContextMenu.IsOpen = true;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            btnExport.ContextMenu.DataContext = btnExport.DataContext;
            btnExport.ContextMenu.IsOpen = true;
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
