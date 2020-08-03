using System.Collections.Generic;

namespace ERHMS.EpiInfo.Projects
{
    public class IncidentProject : Project
    {
        public new static IncidentProject Create(ProjectCreationInfo info)
        {
            return Create<IncidentProject>(info);
        }

        public override ProjectType Type => ProjectType.Incident;
        protected override ICollection<string> BuiltInViewNames { get; } = new string[]
        {
            "WorkerStatus"
        };

        public IncidentProject() { }

        public IncidentProject(string path)
            : base(path) { }
    }
}
