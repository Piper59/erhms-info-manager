using System.Collections.Generic;

namespace ERHMS.EpiInfo.Projects
{
    public class WorkerProject : Project
    {
        public new static WorkerProject Create(ProjectCreationInfo info)
        {
            return Create<WorkerProject>(info);
        }

        public override ProjectType Type => ProjectType.Worker;
        protected override ICollection<string> BuiltInViewNames { get; } = new string[]
        {
            "WorkerInfo"
        };

        public WorkerProject() { }

        public WorkerProject(string path)
            : base(path) { }
    }
}
