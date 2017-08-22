using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
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
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Jobs", "JobId", "ERHMS_JobTeams"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo
                {
                    JoinType = JoinType.Inner,
                    TableNameTo = "ERHMS_Incidents",
                    AliasTo = "ERHMS_JobIncidents",
                    ColumnNameTo = "IncidentId",
                    TableNameOrAliasFrom = "ERHMS_Jobs"
                });
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Teams", "TeamId", "ERHMS_JobTeams"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo
                {
                    JoinType = JoinType.Inner,
                    TableNameTo = "ERHMS_Incidents",
                    AliasTo = "ERHMS_TeamIncidents",
                    ColumnNameTo = "IncidentId",
                    TableNameOrAliasFrom = "ERHMS_Teams"
                });
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
            string clauses = "WHERE [ERHMS_Jobs].[IncidentId] = @IncidentId OR [ERHMS_Teams].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobTeam> SelectByJobId(string jobId)
        {
            string clauses = "WHERE [ERHMS_JobTeams].[JobId] = @JobId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            return Select(clauses, parameters);
        }
    }
}
