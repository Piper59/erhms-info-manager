using System.Collections.Generic;

namespace ERHMS.EpiInfo.Projects
{
    public class CoreView
    {
        public static readonly CoreView WorkerInfo = new CoreView(ProjectType.Worker, nameof(WorkerInfo));
        public static readonly CoreView WorkerStatus = new CoreView(ProjectType.Incident, nameof(WorkerStatus));
        public static readonly IReadOnlyCollection<CoreView> All = new CoreView[]
        {
            WorkerInfo,
            WorkerStatus
        };

        public ProjectType ProjectType { get; }
        public string Name { get; }

        private CoreView(ProjectType projectType, string name)
        {
            ProjectType = projectType;
            Name = name;
        }
    }
}
