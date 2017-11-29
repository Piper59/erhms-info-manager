using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
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
            : base(context.Database) { }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_JobLocations");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.JobId", "ERHMS_JobLocations.JobId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Jobs.IncidentId", "ERHMS_JobIncidents");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Locations.LocationId", "ERHMS_JobLocations.LocationId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Locations.IncidentId", "ERHMS_LocationIncidents");
            return sql;
        }

        private JobLocation Map(JobLocation jobLocation, Job job, Incident jobIncident, Location location, Incident locationIncident)
        {
            jobLocation.Job = job;
            job.Incident = jobIncident;
            jobLocation.Location = location;
            location.Incident = locationIncident;
            return jobLocation;
        }

        public override IEnumerable<JobLocation> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<JobLocation, Job, Incident, Location, Incident, JobLocation>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override JobLocation SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_JobLocations].[JobLocationId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
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
