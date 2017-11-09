using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobRepository : LinkRepository<Job>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Job))
            {
                TableName = "ERHMS_Jobs"
            };
            typeMap.Get(nameof(Job.JobId)).SetId();
            typeMap.Get(nameof(Job.New)).SetComputed();
            typeMap.Get(nameof(Job.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Job), typeMap);
        }

        public JobRepository(DataContext context)
            : base(context) { }

        public IEnumerable<Job> SelectByIncidentIdAndDateRange(string incidentId, DateTime? start, DateTime? end)
        {
            ICollection<string> clauses = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            clauses.Add("[ERHMS_Jobs].[IncidentId] = @IncidentId");
            parameters.Add("@IncidentId", incidentId);
            if (start.HasValue)
            {
                clauses.Add("([ERHMS_Jobs].[EndDate] IS NULL OR [ERHMS_Jobs].[EndDate] >= @Start)");
                parameters.Add("@Start", start.Value.RemoveMilliseconds());
            }
            if (end.HasValue)
            {
                clauses.Add("([ERHMS_Jobs].[StartDate] IS NULL OR [ERHMS_Jobs].[StartDate] <= @End)");
                parameters.Add("@End", start.Value.RemoveMilliseconds());
            }
            return Select(string.Format("WHERE {0}", string.Join(" AND ", clauses)), parameters);
        }
    }
}
