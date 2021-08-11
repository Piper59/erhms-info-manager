using ERHMS.Desktop.ViewModels;
using System.Windows;

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
    }
}
