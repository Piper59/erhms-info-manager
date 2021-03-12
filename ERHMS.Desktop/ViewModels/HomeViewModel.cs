using ERHMS.Desktop.ViewModels.Collections;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public PhaseCollectionViewModel Phases { get; }

        public HomeViewModel()
        {
            Phases = new PhaseCollectionViewModel();
        }
    }
}
