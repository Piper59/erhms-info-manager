namespace ERHMS.EpiInfo.DataAccess
{
    public class RepositoryBase
    {
        protected IDataDriver Driver { get; private set; }

        protected RepositoryBase(IDataDriver driver)
        {
            Driver = driver;
        }
    }
}
