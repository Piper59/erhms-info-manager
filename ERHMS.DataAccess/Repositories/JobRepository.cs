using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class JobRepository : IncidentEntityRepository<Job>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Job))
            {
                TableName = "ERHMS_Jobs"
            };
            typeMap.Get(nameof(Job.New)).SetComputed();
            typeMap.Get(nameof(Job.Id)).SetComputed();
            typeMap.Get(nameof(Job.Guid)).SetComputed();
            typeMap.Get(nameof(Job.JobId)).SetId();
            typeMap.Get(nameof(Job.Incident)).SetComputed();
            typeMap.Get(nameof(Job.Responders)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Job), typeMap);
        }

        public JobRepository(DataContext context)
            : base(context) { }

        public IEnumerable<Job> SelectByIncidentIdAndDateRange(string incidentId, DateTime? startDate, DateTime? endDate)
        {
            ICollection<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            conditions.Add("[ERHMS_Jobs].[IncidentId] = @IncidentId");
            parameters.Add("@IncidentId", incidentId);
            if (startDate.HasValue)
            {
                conditions.Add("([ERHMS_Jobs].[EndDate] IS NULL OR [ERHMS_Jobs].[EndDate] >= @StartDate)");
                parameters.Add("@StartDate", startDate.Value.RemoveMilliseconds());
            }
            if (endDate.HasValue)
            {
                conditions.Add("([ERHMS_Jobs].[StartDate] IS NULL OR [ERHMS_Jobs].[StartDate] <= @EndDate)");
                parameters.Add("@EndDate", endDate.Value.RemoveMilliseconds());
            }
            return Select(SqlBuilder.GetWhereClause(conditions), parameters);
        }
    }

    public static class JobRepositoryExtensions
    {
        public static IEnumerable<Job> WithResponders(this IEnumerable<Job> @this, DataContext context, bool undeleted = true)
        {
            ILookup<string, Team> jobTeams = context.JobTeams.Select().ToLookup(
                jobTeam => jobTeam.JobId,
                jobTeam => jobTeam.Team,
                StringComparer.OrdinalIgnoreCase);
            ILookup<string, Responder> teamResponders = context.TeamResponders.Select().ToLookup(
                teamResponder => teamResponder.TeamId,
                teamResponder => teamResponder.Responder,
                StringComparer.OrdinalIgnoreCase);
            ILookup<string, Responder> jobResponders = context.JobResponders.Select().ToLookup(
                jobResponder => jobResponder.JobId,
                jobResponder => jobResponder.Responder,
                StringComparer.OrdinalIgnoreCase);
            foreach (Job job in @this)
            {
                ISet<Responder> responders = new HashSet<Responder>();
                foreach (Team team in jobTeams[job.JobId])
                {
                    responders.AddRange(teamResponders[team.TeamId]);
                }
                responders.AddRange(jobResponders[job.JobId]);
                job.Responders = responders.Where(responder => !responder.Deleted || !undeleted).ToList();
                yield return job;
            }
        }
    }
}
