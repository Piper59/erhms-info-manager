using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
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
            : base(context.Database) { }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_JobTeams");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.JobId", "ERHMS_JobTeams.JobId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Jobs.IncidentId", "ERHMS_JobIncidents");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Teams.TeamId", "ERHMS_JobTeams.TeamId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Teams.IncidentId", "ERHMS_TeamIncidents");
            return sql;
        }

        private JobTeam Map(JobTeam jobTeam, Job job, Incident jobIncident, Team team, Incident teamIncident)
        {
            jobTeam.Job = job;
            job.Incident = jobIncident;
            jobTeam.Team = team;
            team.Incident = teamIncident;
            return jobTeam;
        }

        public override IEnumerable<JobTeam> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<JobTeam, Job, Incident, Team, Incident, JobTeam>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override JobTeam SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobTeams].[JobTeamId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobTeam> SelectByIncidentIdAndDateRange(string incidentId, DateTime? startDate, DateTime? endDate)
        {
            ICollection<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            conditions.Add("([ERHMS_Jobs].[IncidentId] = @JobIncidentId OR [ERHMS_Teams].[IncidentId] = @TeamIncidentId)");
            parameters.Add("@JobIncidentId", incidentId);
            parameters.Add("@TeamIncidentId", incidentId);
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
