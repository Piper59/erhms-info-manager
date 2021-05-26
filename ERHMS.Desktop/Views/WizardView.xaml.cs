using ERHMS.Desktop.ViewModels;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class WizardView : Window
    {
        public new WizardViewModel DataContext
        {
            get { return (WizardViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public WizardView()
        {
            InitializeComponent();
            DataContextChanged += WizardView_DataContextChanged;
        }

        private void WizardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((WizardViewModel)e.OldValue).CloseRequested -= DataContext_CloseRequested;
            }
            if (e.NewValue != null)
            {
                ((WizardViewModel)e.NewValue).CloseRequested += DataContext_CloseRequested;
            }
        }

        private void DataContext_CloseRequested(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
