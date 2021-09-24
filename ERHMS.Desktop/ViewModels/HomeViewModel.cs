using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel
    {
        public class PhaseViewModel
        {
            public Phase Phase { get; }
            public ProjectInfoCollectionViewModel ProjectInfos { get; }
            public IEnumerable<CoreView> CoreViews { get; }

            public PhaseViewModel(Phase phase, ProjectInfoCollectionViewModel projectInfos)
            {
                Phase = phase;
                ProjectInfos = projectInfos;
                CoreViews = CoreView.GetInstances(phase).ToList();
            }
        }

        public IEnumerable<PhaseViewModel> Phases { get; }

        public HomeViewModel()
        {
            ProjectInfoCollectionViewModel workerProjectInfos = new ProjectInfoCollectionViewModel.Workers();
            ProjectInfoCollectionViewModel incidentProjectInfos = new ProjectInfoCollectionViewModel.Incidents();
            Phases = new PhaseViewModel[]
            {
                new PhaseViewModel(Phase.PreDeployment, workerProjectInfos),
                new PhaseViewModel(Phase.Deployment, incidentProjectInfos),
                new PhaseViewModel(Phase.PostDeployment, incidentProjectInfos)
            };
        }
    }
}
