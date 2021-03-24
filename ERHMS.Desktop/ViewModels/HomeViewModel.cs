using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public PhaseCollectionViewModel Phases { get; } = new PhaseCollectionViewModel();
    }
}
