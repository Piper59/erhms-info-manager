using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class LocationRepository : TableEntityRepository<Location>
    {
        public LocationRepository(IDataDriver driver)
            : base(driver, "ERHMS_Locations") { }

        public IEnumerable<Location> SelectByIncidentId(string incidentId)
        {
            return Select("[IncidentId] = {@}", incidentId);
        }

        public IEnumerable<Location> SelectByJobId(string jobId)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.Append(@"
                SELECT [ERHMS_Locations].*
                FROM [ERHMS_Locations]
                INNER JOIN [ERHMS_JobLocations] ON [ERHMS_Locations].[LocationId] = [ERHMS_JobLocations].[LocationId]
                WHERE [ERHMS_JobLocations].[JobId] = {@}");
            builder.Values.Add(jobId);
            return Mapper.Create(Driver.ExecuteQuery(builder.GetQuery()));
        }
    }
}
