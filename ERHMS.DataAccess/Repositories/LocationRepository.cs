using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class LocationRepository : IncidentEntityRepository<Location>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Location))
            {
                TableName = "ERHMS_Locations"
            };
            typeMap.Get(nameof(Location.New)).SetComputed();
            typeMap.Get(nameof(Location.Id)).SetComputed();
            typeMap.Get(nameof(Location.Guid)).SetComputed();
            typeMap.Get(nameof(Location.LocationId)).SetId();
            typeMap.Get(nameof(Location.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Location), typeMap);
        }

        public LocationRepository(DataContext context)
            : base(context) { }

        public IEnumerable<Location> SelectJobbable(string incidentId, string jobId)
        {
            string clauses = @"
                WHERE [ERHMS_Locations].[LocationId] NOT IN (
                    SELECT [LocationId]
                    FROM [ERHMS_JobLocations]
                    WHERE [JobId] = @JobId
                )
                AND [ERHMS_Locations].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }
    }
}
