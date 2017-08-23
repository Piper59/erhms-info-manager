using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class JobLocationRepository : EntityRepository<JobLocation>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(JobLocation))
            {
                TableName = "ERHMS_JobLocations"
            };
            typeMap.Get(nameof(JobLocation.JobLocationId)).SetId();
            typeMap.Get(nameof(JobLocation.New)).SetComputed();
            typeMap.Get(nameof(JobLocation.Job)).SetComputed();
            typeMap.Get(nameof(JobLocation.Location)).SetComputed();
            SqlMapper.SetTypeMap(typeof(JobLocation), typeMap);
        }

        public JobLocationRepository(DataContext context)
            : base(context) { }

        public override IEnumerable<JobLocation> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_JobLocations");
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Jobs", "JobId", "ERHMS_JobLocations"));
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
                sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Locations", "LocationId", "ERHMS_JobLocations"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo
                {
                    JoinType = JoinType.Inner,
                    TableNameTo = "ERHMS_Incidents",
                    AliasTo = "ERHMS_LocationIncidents",
                    ColumnNameTo = "IncidentId",
                    TableNameOrAliasFrom = "ERHMS_Locations"
                });
                sql.OtherClauses = clauses;
                Func<JobLocation, Job, Incident, Location, Incident, JobLocation> map = (jobLocation, job, jobIncident, location, locationIncident) =>
                {
                    jobLocation.Job = job;
                    job.Incident = jobIncident;
                    jobLocation.Location = location;
                    location.Incident = locationIncident;
                    return jobLocation;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<JobLocation> SelectUndeleted()
        {
            return Select("WHERE [ERHMS_JobIncidents].[Deleted] = 0 AND [ERHMS_LocationIncidents].[Deleted] = 0");
        }

        public override JobLocation SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobLocations].[JobLocationId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<JobLocation> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Jobs].[IncidentId] = @IncidentId OR [ERHMS_Locations].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobLocation> SelectByJobId(string jobId)
        {
            string clauses = "WHERE [ERHMS_JobLocations].[JobId] = @JobId";
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

        public void DeleteByLocationId(string locationId)
        {
            string clauses = "WHERE [LocationId] = @LocationId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LocationId", locationId);
            Delete(clauses, parameters);
        }
    }
}
