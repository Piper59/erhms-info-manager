using ERHMS.Domain;
using ERHMS.EpiInfo.Domain;
using ERHMS.Presentation.ViewModel;

namespace ERHMS.Presentation.View
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
