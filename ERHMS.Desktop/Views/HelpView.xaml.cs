using ERHMS.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        private void FlowDocumentScrollViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FlowDocumentScrollViewer documentViewer = (FlowDocumentScrollViewer)sender;
            if (documentViewer.Template.FindName("PART_ContentHost", documentViewer) is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToTop();
            }
        }
    }
}
