using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class TeamRepository : TableEntityRepository<Team>
    {
        public TeamRepository(IDataDriver driver)
            : base(driver, "ERHMS_Teams") { }

        public IEnumerable<Team> SelectByIncidentId(string incidentId)
        {
            return Select("[IncidentId] = {@}", incidentId);
        }

        public IEnumerable<Team> SelectByJobId(string jobId)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.Append(@"
                SELECT [ERHMS_Teams].*
                FROM [ERHMS_Teams]
                INNER JOIN [ERHMS_JobTeams] ON [ERHMS_Teams].[TeamId] = [ERHMS_JobTeams].[TeamId]
                WHERE [ERHMS_JobTeams].[JobId] = {@}");
            builder.Values.Add(jobId);
            return Mapper.Create(Driver.ExecuteQuery(builder.GetQuery()));
        }
    }
}
