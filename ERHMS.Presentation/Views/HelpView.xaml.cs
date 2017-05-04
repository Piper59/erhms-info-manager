using System.Windows.Controls;

namespace ERHMS.Presentation.Views
{
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl.SelectedIndex = 0;
        }
    }
}
