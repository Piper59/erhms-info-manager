using ERHMS.Desktop.ViewModels;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class StartView : UserControl
    {
        public new StartViewModel DataContext
        {
            get { return (StartViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public StartView()
        {
            InitializeComponent();
        }
    }
}
