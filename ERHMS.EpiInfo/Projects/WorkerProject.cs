using Epi;

namespace ERHMS.EpiInfo.Projects
{
    public class WorkerProject : Project
    {
        public new static WorkerProject Create(ProjectCreationInfo info)
        {
            return Create<WorkerProject>(info);
        }

        public override ProjectType Type => ProjectType.Worker;
        public View WorkerInfoView => Views[CoreView.WorkerInfo.Name];

        public WorkerProject() { }

        public WorkerProject(string path)
            : base(path) { }
    }
}
