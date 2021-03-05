using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERHMS.Domain
{
    public class CoreView
    {
        public static CoreView WorkerRosteringForm { get; } = new CoreView(
            CoreProject.Worker,
            Phase.PreDeployment,
            "WorkerRosteringForm",
            "Worker Rostering Form");

        public static CoreView PreDeploymentHealthSurvey { get; } = new CoreView(
            CoreProject.Worker,
            Phase.PreDeployment,
            "PreDeploymentHealthSurvey",
            "Pre-Deployment Health Survey");

        public static CoreView WorkerDeploymentRecord { get; } = new CoreView(
            CoreProject.Incident,
            Phase.Deployment,
            "WorkerDeploymentRecord",
            "Worker Deployment Record");

        public static CoreView WorkerInProcessingForm { get; } = new CoreView(
            CoreProject.Incident,
            Phase.Deployment,
            "WorkerInProcessingForm",
            "Worker In-Processing Form");

        public static CoreView WorkerActivityReport { get; } = new CoreView(
            CoreProject.Incident,
            Phase.Deployment,
            "WorkerActivityReport",
            "Worker Activity Report");

        public static CoreView DeploymentHealthSurvey { get; } = new CoreView(
            CoreProject.Incident,
            Phase.Deployment,
            "DeploymentHealthSurvey",
            "Deployment Health Survey");

        public static CoreView WorkerOutProcessingForm { get; } = new CoreView(
            CoreProject.Incident,
            Phase.PostDeployment,
            "WorkerOutProcessingForm",
            "Worker Out-Processing Form");

        public static CoreView PostDeploymentHealthSurvey { get; } = new CoreView(
            CoreProject.Incident,
            Phase.PostDeployment,
            "PostDeploymentHealthSurvey",
            "Post-Deployment Health Survey");

        public static CoreView AfterActionReview { get; } = new CoreView(
            CoreProject.Incident,
            Phase.PostDeployment,
            "AfterActionReview",
            "After-Action Review");

        public static IReadOnlyCollection<CoreView> Instances { get; } = typeof(CoreView)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(CoreView))
            .Select(property => (CoreView)property.GetValue(null))
            .ToList();

        public CoreProject CoreProject { get; }
        public Phase Phase { get; }
        public string Name { get; }
        public string Title { get; }

        private CoreView(CoreProject coreProject, Phase phase, string name, string title)
        {
            CoreProject = coreProject;
            Phase = phase;
            Name = name;
            Title = title;
        }
    }
}
