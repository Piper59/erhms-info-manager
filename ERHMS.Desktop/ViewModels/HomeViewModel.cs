using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        public IReadOnlyCollection<PhaseViewModel> Phases { get; } = new PhaseViewModel[]
        {
            new PhaseViewModel(Phase.PreDeployment),
            new PhaseViewModel(Phase.Deployment),
            new PhaseViewModel(Phase.PostDeployment)
        };
    }
}
