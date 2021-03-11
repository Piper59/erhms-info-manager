using ERHMS.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.ViewModels
{
    public class PhaseViewModel : ViewModel
    {
        public static PhaseViewModel PreDeployment { get; } = new PhaseViewModel(Phase.PreDeployment);
        public static PhaseViewModel Deployment { get; } = new PhaseViewModel(Phase.Deployment);
        public static PhaseViewModel PostDeployment { get; } = new PhaseViewModel(Phase.PostDeployment);

        public static IReadOnlyCollection<PhaseViewModel> Instances { get; } = new PhaseViewModel[]
        {
            PreDeployment,
            Deployment,
            PostDeployment
        };

        public Phase Value { get; }
        public CoreProject CoreProject { get; }
        public IReadOnlyCollection<CoreView> CoreViews { get; }

        private PhaseViewModel(Phase value)
        {
            Value = value;
            CoreProject = value.ToCoreProject();
            CoreViews = CoreView.GetInstances(value).ToList();
        }
    }
}
