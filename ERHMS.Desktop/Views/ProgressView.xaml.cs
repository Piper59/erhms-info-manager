using ERHMS.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class ProgressView : Window
    {
        public new ProgressViewModel DataContext
        {
            get { return (ProgressViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ProgressView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataContext.Done)
            {
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
