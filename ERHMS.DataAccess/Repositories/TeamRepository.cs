using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class TeamRepository : LinkRepository<Team>
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
}
