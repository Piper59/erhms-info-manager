using System.Windows.Controls;
using ERHMS.WPF.ViewModel;

namespace ERHMS.WPF.View
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
