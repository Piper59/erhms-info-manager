using ERHMS.Domain;
using ERHMS.EpiInfo.Domain;
using ERHMS.WPF.ViewModel;

namespace ERHMS.WPF.View
{
    /// <summary>
    /// Interaction logic for ResponderView.xaml
    /// </summary>
    public partial class ResponderView
    {
        public ResponderView()
        {
            InitializeComponent();
            DataContext = new ResponderViewModel();
        }

        public ResponderView(Responder responder)
        {
            InitializeComponent();
            DataContext = new ResponderViewModel(responder);
        }
    }
}
