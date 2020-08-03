using ERHMS.Desktop.ViewModels;
using System;
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
            if (!DataContext.Complete)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void DataContext_Completed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
