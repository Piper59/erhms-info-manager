using Dapper;
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

        public DataContext Context { get; private set; }

        public JobResponderRepository(DataContext context)
            : base(context.Database)
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
                sql.AddTable(JoinType.Inner, "ERHMS_Jobs", "ERHMS_JobResponders", "JobId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_JobIncidents");
                sql.FromClauses.Add("INNER JOIN [ERHMS_Incidents] AS [ERHMS_JobIncidents] ON [ERHMS_Jobs].[IncidentId] = [ERHMS_JobIncidents].[IncidentId]");
                sql.AddSeparator();
                foreach (string tableName in Context.Responders.TableNames)
                {
                    sql.AddTableSelectClause(tableName);
                    sql.FromClauses.Add(string.Format("INNER JOIN {0} ON [ERHMS_JobResponders].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName)));
                }
                sql.AddSeparator();
                sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles", "ERHMS_JobResponders", "IncidentRoleId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_IncidentRoleIncidents");
                sql.FromClauses.Add("LEFT OUTER JOIN [ERHMS_Incidents] AS [ERHMS_IncidentRoleIncidents] ON [ERHMS_IncidentRoles].[IncidentId] = [ERHMS_IncidentRoleIncidents].[IncidentId]");
                sql.OtherClauses = clauses;
                Func<JobResponder, Job, Incident, Responder, IncidentRole, Incident, JobResponder> map = (jobResponder, job, jobIncident, responder, incidentRole, incidentRoleIncident) =>
                {
                    jobResponder.Job = job;
                    job.Incident = jobIncident;
                    jobResponder.Responder = responder;
                    if (incidentRole.IncidentRoleId != null)
                    {
                        jobResponder.IncidentRole = incidentRole;
                        incidentRole.Incident = incidentRoleIncident;
                    }
                    return jobResponder;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<JobResponder> SelectUndeleted()
        {
            string format = @"
                WHERE [ERHMS_JobIncidents].[Deleted] = 0
                AND ([ERHMS_IncidentRoleIncidents].[Deleted] IS NULL OR [ERHMS_IncidentRoleIncidents].[Deleted] = 0)
                AND {0}.[RECSTATUS] <> 0";
            return Select(string.Format(format, Escape(Context.Responders.View.TableName)));
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
            parameters.Add("@JobIncidentId", incidentId);
            parameters.Add("@IncidentRoleIncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobResponder> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Jobs].[IncidentId] = @JobIncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentRoleIncidentId";
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        public IEnumerable<JobResponder> SelectUndeletedByIncidentId(string incidentId)
        {
            string format = @"
                WHERE ([ERHMS_Jobs].[IncidentId] = @JobIncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentRoleIncidentId)
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(Context.Responders.View.TableName));
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

        public void DeleteByJobId(string jobId)
        {
            string clauses = "WHERE [JobId] = @JobId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            Delete(clauses, parameters);
        }
    }
}
