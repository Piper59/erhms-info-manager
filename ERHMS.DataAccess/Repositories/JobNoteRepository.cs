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
    public class JobNoteRepository : EntityRepository<JobNote>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(JobNote))
            {
                TableName = "ERHMS_JobNotes"
            };
            typeMap.Get(nameof(JobNote.New)).SetComputed();
            typeMap.Get(nameof(JobNote.Id)).SetComputed();
            typeMap.Get(nameof(JobNote.Guid)).SetComputed();
            typeMap.Get(nameof(JobNote.JobNoteId)).SetId();
            typeMap.Get(nameof(JobNote.Job)).SetComputed();
            SqlMapper.SetTypeMap(typeof(JobNote), typeMap);
        }

        public JobNoteRepository(DataContext context)
            : base(context.Database) { }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_JobNotes");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.JobId", "ERHMS_JobNotes.JobId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Jobs.IncidentId");
            return sql;
        }

        private JobNote Map(JobNote jobNote, Job job, Incident incident)
        {
            jobNote.Job = job;
            job.Incident = incident;
            return jobNote;
        }

        public override IEnumerable<JobNote> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<JobNote, Job, Incident, JobNote>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override JobNote SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobNotes].[JobNoteId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobNote> SelectByIncidentIdAndDateRange(string incidentId, DateTime? startDate, DateTime? endDate)
        {
            ICollection<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            conditions.Add("[ERHMS_Jobs].[IncidentId] = @IncidentId");
            parameters.Add("@IncidentId", incidentId);
            if (startDate.HasValue)
            {
                conditions.Add("[ERHMS_JobNotes].[Date] >= @StartDate");
                parameters.Add("@StartDate", startDate.Value.RemoveMilliseconds());
            }
            if (endDate.HasValue)
            {
                conditions.Add("[ERHMS_JobNotes].[Date] <= @EndDate");
                parameters.Add("@EndDate", endDate.Value.RemoveMilliseconds());
            }
            return Select(SqlBuilder.GetWhereClause(conditions), parameters);
        }

        public IEnumerable<JobNote> SelectByJobId(string jobId)
        {
            string clauses = "WHERE [ERHMS_JobNotes].[JobId] = @JobId";
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
