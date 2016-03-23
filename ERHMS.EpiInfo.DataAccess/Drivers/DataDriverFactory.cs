using System;

namespace ERHMS.EpiInfo.DataAccess
{
    public static class DataDriverFactory
    {
        public static IDataDriver CreateDataDriver(Project project)
        {
            DataProvider provider = DataProviderExtensions.FromEpiInfoName(project.CollectedDataDriver);
            switch (provider)
            {
                case DataProvider.Access:
                    return AccessDriver.Create(project);
                case DataProvider.SqlServer:
                    return SqlServerDriver.Create(project);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
