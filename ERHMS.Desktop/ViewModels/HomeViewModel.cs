using ERHMS.Desktop.Collections;
using ERHMS.Desktop.Infrastructure.ViewModels;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public PhaseCollection Phases { get; } = new PhaseCollection();
    }
}
