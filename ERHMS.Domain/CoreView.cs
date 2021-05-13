using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ERHMS.Domain
{
    public class CoreView
    {
        public static CoreView WorkerRosteringForm { get; } = new CoreView(Phase.PreDeployment);
        public static CoreView PreDeploymentHealthSurvey { get; } = new CoreView(Phase.PreDeployment);
        public static CoreView WorkerDeploymentRecord { get; } = new CoreView(Phase.Deployment);
        public static CoreView WorkerInProcessingForm { get; } = new CoreView(Phase.Deployment);
        public static CoreView WorkerActivityReport { get; } = new CoreView(Phase.Deployment);
        public static CoreView DeploymentHealthSurvey { get; } = new CoreView(Phase.Deployment);
        public static CoreView WorkerOutProcessingForm { get; } = new CoreView(Phase.PostDeployment);
        public static CoreView PostDeploymentHealthSurvey { get; } = new CoreView(Phase.PostDeployment);
        public static CoreView AfterActionReview { get; } = new CoreView(Phase.PostDeployment);

        public static IEnumerable<CoreView> Instances { get; } = new CoreView[]
        {
            WorkerRosteringForm,
            PreDeploymentHealthSurvey,
            WorkerDeploymentRecord,
            WorkerInProcessingForm,
            WorkerActivityReport,
            DeploymentHealthSurvey,
            WorkerOutProcessingForm,
            PostDeploymentHealthSurvey,
            AfterActionReview
        };

        public CoreProject CoreProject => Phase.ToCoreProject();
        public Phase Phase { get; }
        public string Name { get; }

        private CoreView(Phase phase, [CallerMemberName] string name = null)
        {
            Phase = phase;
            Name = name;
        }
    }
}
