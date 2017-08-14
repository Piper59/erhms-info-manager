using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class JobResponderRepository : EntityRepository<JobResponder>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(JobResponder))
            {
                TableName = "ERHMS_JobResponders"
            };
            typeMap.Get(nameof(JobResponder.JobResponderId)).SetId();
            typeMap.Get(nameof(JobResponder.New)).SetComputed();
            typeMap.Get(nameof(JobResponder.Job)).SetComputed();
            typeMap.Get(nameof(JobResponder.Responder)).SetComputed();
            typeMap.Get(nameof(JobResponder.IncidentRole)).SetComputed();
            SqlMapper.SetTypeMap(typeof(JobResponder), typeMap);
        }

        public new DataContext Context { get; private set; }

        public JobResponderRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override IEnumerable<JobResponder> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_JobResponders");
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Jobs", "JobId", "ERHMS_JobResponders"));
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
                foreach (string tableName in Context.Responders.TableNames)
                {
                    sql.AddTable(new JoinInfo(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_JobResponders", "ResponderId"));
                }
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_IncidentRoles", "IncidentRoleId", "ERHMS_JobResponders"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo
                {
                    JoinType = JoinType.Inner,
                    TableNameTo = "ERHMS_Incidents",
                    AliasTo = "ERHMS_IncidentRoleIncidents",
                    ColumnNameTo = "IncidentId",
                    TableNameOrAliasFrom = "ERHMS_IncidentRoles"
                });
                sql.OtherClauses = clauses;
                Func<JobResponder, Job, Incident, Responder, IncidentRole, Incident, JobResponder> map = (jobResponder, job, jobIncident, responder, incidentRole, incidentRoleIncident) =>
                {
                    SetOld(jobResponder, job, jobIncident, responder, incidentRole, incidentRoleIncident);
                    jobResponder.Job = job;
                    job.Incident = jobIncident;
                    jobResponder.Responder = responder;
                    jobResponder.IncidentRole = incidentRole;
                    incidentRole.Incident = incidentRoleIncident;
                    return jobResponder;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<JobResponder> SelectUndeleted()
        {
            return Select(string.Format(
                "WHERE [ERHMS_JobIncidents].[Deleted] = 0 AND [ERHMS_IncidentRoleIncidents].[Deleted] = 0 AND {0}.[REC_STATUS] <> 0",
                Escape(Context.Responders.View.TableName)));
        }

        public override JobResponder SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobResponders].[JobResponderId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        private IEnumerable<JobResponder> SelectByIncidentIdInternal(string clauses, string incidentId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobResponder> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Jobs].[IncidentId] = @IncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentId";
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        public IEnumerable<JobResponder> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE ([ERHMS_Jobs].[IncidentId] = @IncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentId) AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        private IEnumerable<JobResponder> SelectByJobIdInternal(string clauses, string teamId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobResponder> SelectByJobId(string jobId)
        {
            return SelectByJobIdInternal("WHERE [ERHMS_JobResponders].[JobId] = @JobId", jobId);
        }

        public IEnumerable<JobResponder> SelectUndeletedByJobId(string jobId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_JobResponders].[JobId] = @JobId AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            return SelectByJobIdInternal(clauses, jobId);
        }
    }
}
