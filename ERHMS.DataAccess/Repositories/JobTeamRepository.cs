using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class JobTeamRepository : EntityRepository<JobTeam>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(JobTeam))
            {
                TableName = "ERHMS_JobTeams"
            };
            typeMap.Get(nameof(JobTeam.JobTeamId)).SetId();
            typeMap.Get(nameof(JobTeam.New)).SetComputed();
            typeMap.Get(nameof(JobTeam.Job)).SetComputed();
            typeMap.Get(nameof(JobTeam.Team)).SetComputed();
            SqlMapper.SetTypeMap(typeof(JobTeam), typeMap);
        }

        public JobTeamRepository(DataContext context)
            : base(context) { }

        public override IEnumerable<JobTeam> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_JobTeams");
                sql.AddSeparator();
                sql.AddTable(JoinType.Inner, "ERHMS_Jobs", "ERHMS_JobTeams", "JobId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_JobIncidents");
                sql.FromClauses.Add("INNER JOIN [ERHMS_Incidents] AS [ERHMS_JobIncidents] ON [ERHMS_Jobs].[IncidentId] = [ERHMS_JobIncidents].[IncidentId]");
                sql.AddSeparator();
                sql.AddTable(JoinType.Inner, "ERHMS_Teams", "ERHMS_JobTeams", "TeamId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_TeamIncidents");
                sql.FromClauses.Add("INNER JOIN [ERHMS_Incidents] AS [ERHMS_TeamIncidents] ON [ERHMS_Teams].[IncidentId] = [ERHMS_TeamIncidents].[IncidentId]");
                sql.OtherClauses = clauses;
                Func<JobTeam, Job, Incident, Team, Incident, JobTeam> map = (jobTeam, job, jobIncident, team, teamIncident) =>
                {
                    jobTeam.Job = job;
                    job.Incident = jobIncident;
                    jobTeam.Team = team;
                    team.Incident = teamIncident;
                    return jobTeam;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<JobTeam> SelectUndeleted()
        {
            return Select("WHERE [ERHMS_JobIncidents].[Deleted] = 0 AND [ERHMS_TeamIncidents].[Deleted] = 0");
        }

        public override JobTeam SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobTeams].[JobTeamId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobTeam> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Jobs].[IncidentId] = @JobIncidentId OR [ERHMS_Teams].[IncidentId] = @TeamIncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobIncidentId", incidentId);
            parameters.Add("@TeamIncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobTeam> SelectByIncidentIdAndDateRange(string incidentId, DateTime? start, DateTime? end)
        {
            ICollection<string> clauses = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            clauses.Add("([ERHMS_Jobs].[IncidentId] = @JobIncidentId OR [ERHMS_Teams].[IncidentId] = @TeamIncidentId)");
            parameters.Add("@JobIncidentId", incidentId);
            parameters.Add("@TeamIncidentId", incidentId);
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

        public IEnumerable<JobTeam> SelectByJobId(string jobId)
        {
            string clauses = "WHERE [ERHMS_JobTeams].[JobId] = @JobId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            return Select(clauses, parameters);
        }

        public void DeleteByJobId(string jobId)
        {
            string clauses = "WHERE [JobId] = @JobId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            Delete(clauses, parameters);
        }

        public void DeleteByTeamId(string teamId)
        {
            string clauses = "WHERE [TeamId] = @TeamId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            Delete(clauses, parameters);
        }
    }
}
