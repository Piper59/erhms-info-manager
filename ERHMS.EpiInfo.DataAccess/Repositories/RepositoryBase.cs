using Epi;

namespace ERHMS.EpiInfo.DataAccess
{
    public class RepositoryBase
    {
        public IDataDriver Driver { get; private set; }

        public Project Project
        {
            get { return Driver.Project; }
        }

        protected RepositoryBase(IDataDriver driver)
        {
            Driver = driver;
        }
    }
}
