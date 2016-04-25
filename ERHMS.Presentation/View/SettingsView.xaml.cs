using System.Windows.Controls;
using ERHMS.Presentation.ViewModel;

namespace ERHMS.Presentation.View
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel();
        }
    }
}
