using Epi;
using Epi.Data;

namespace ERHMS.EpiInfo.Data.Repositories
{
    public abstract class RepositoryBase
    {
        public Project Project { get; private set; }

        protected IDbDriver Driver
        {
            get { return Project.CollectedData.GetDbDriver(); }
        }

        protected RepositoryBase(Project project)
        {
            Project = project;
        }
    }
}
