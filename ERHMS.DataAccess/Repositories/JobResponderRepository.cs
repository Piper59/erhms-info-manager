using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
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

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_JobResponders");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.JobId", "ERHMS_JobResponders.JobId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Jobs.IncidentId", "ERHMS_JobIncidents");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_JobResponders", "ResponderId");
            }
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles.IncidentRoleId", "ERHMS_JobResponders.IncidentRoleId");
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents.IncidentId", "ERHMS_IncidentRoles.IncidentId", "ERHMS_IncidentRoleIncidents");
            return sql;
        }

        private JobResponder Map(
            JobResponder jobResponder,
            Job job,
            Incident jobIncident,
            Responder responder,
            IncidentRole incidentRole,
            Incident incidentRoleIncident)
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
        }

        public override IEnumerable<JobResponder> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<JobResponder, Job, Incident, Responder, IncidentRole, Incident, JobResponder>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override JobResponder SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobResponders].[JobResponderId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobResponder> SelectUndeletedByJobId(string jobId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_JobResponders].[JobId] = @JobId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
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
    }
}
