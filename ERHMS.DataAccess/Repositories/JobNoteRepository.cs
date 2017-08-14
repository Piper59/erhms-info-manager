using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
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
            typeMap.Get(nameof(JobNote.JobNoteId)).SetId();
            typeMap.Get(nameof(JobNote.New)).SetComputed();
            typeMap.Get(nameof(JobNote.Job)).SetComputed();
            SqlMapper.SetTypeMap(typeof(JobNote), typeMap);
        }

        public JobNoteRepository(DataContext context)
            : base(context) { }

        public override IEnumerable<JobNote> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_JobNotes");
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Jobs", "JobId", "ERHMS_JobNotes"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Incidents", "IncidentId", "ERHMS_Jobs"));
                sql.OtherClauses = clauses;
                Func<JobNote, Job, Incident, JobNote> map = (jobNote, job, incident) =>
                {
                    SetOld(jobNote, job, incident);
                    jobNote.Job = job;
                    job.Incident = incident;
                    return jobNote;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<JobNote> SelectUndeleted()
        {
            return Select("WHERE [ERHMS_Incidents].[Deleted] = 0");
        }

        public override JobNote SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobNotes].[JobNoteId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobNote> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_JobNotes].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobNote> SelectByJobId(string jobId)
        {
            string clauses = "WHERE [ERHMS_JobNotes].[JobId] = @JobId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            return Select(clauses, parameters);
        }
    }
}
