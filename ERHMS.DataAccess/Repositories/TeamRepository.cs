using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class TeamRepository : IncidentEntityRepository<Team>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Team))
            {
                TableName = "ERHMS_Teams"
            };
            typeMap.Get(nameof(Team.TeamId)).SetId();
            typeMap.Get(nameof(Team.New)).SetComputed();
            typeMap.Get(nameof(Team.Incident)).SetComputed();
            typeMap.Get(nameof(Team.Responders)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Team), typeMap);
        }

        public TeamRepository(DataContext context)
            : base(context) { }

        public IEnumerable<Team> SelectJobbable(string incidentId, string jobId)
        {
            string clauses = @"
                WHERE [ERHMS_Teams].[TeamId] NOT IN (
                    SELECT [TeamId]
                    FROM [ERHMS_JobTeams]
                    WHERE [JobId] = @JobId
                )
                AND [ERHMS_Teams].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }
    }

    public static class TeamRepositoryExtensions
    {
        public static IEnumerable<Team> WithResponders(this IEnumerable<Team> @this, DataContext context, bool undeleted = true)
        {
            ILookup<string, Responder> teamResponders = context.TeamResponders.Select().ToLookup(
                teamResponder => teamResponder.TeamId,
                teamResponder => teamResponder.Responder,
                StringComparer.OrdinalIgnoreCase);
            foreach (Team team in @this)
            {
                team.Responders = teamResponders[team.TeamId].Where(responder => !responder.Deleted || !undeleted).ToList();
                yield return team;
            }
        }
    }
}
