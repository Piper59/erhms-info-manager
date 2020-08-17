using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Wizards;
using System;
using System.ComponentModel;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class WizardView : Window
    {
        public new IWizard DataContext
        {
            get { return (WizardViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public WizardView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !DataContext.ExitCommand.CanExecute(null);
            base.OnClosing(e);
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
