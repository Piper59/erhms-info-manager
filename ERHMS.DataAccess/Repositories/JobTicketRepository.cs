using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class JobTicketRepository : EntityRepository<JobTicket>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(JobTicket));
            SqlMapper.SetTypeMap(typeof(JobTicket), typeMap);
        }

        private static JobTicket Map(Incident incident, Job job, Team team, Responder responder, IncidentRole incidentRole, ILookup<string, Location> locations)
        {
            JobTicket jobTicket = new JobTicket
            {
                Incident = incident,
                Job = job,
                Team = team,
                Responder = responder,
                Locations = locations[job.JobId].ToList()
            };
            if (incidentRole.IncidentRoleId != null)
            {
                jobTicket.IncidentRole = incidentRole;
            }
            return jobTicket;
        }

        public new DataContext Context { get; private set; }

        public JobTicketRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override int Count(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<JobTicket> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                ILookup<string, Location> locations = Context.JobLocations.Select().ToLookup(
                    jobLocation => jobLocation.JobId,
                    jobLocation => jobLocation.Location,
                    StringComparer.OrdinalIgnoreCase);
                IEnumerable<JobTicket> result1;
                {
                    SqlBuilder sql = new SqlBuilder();
                    sql.AddTable("ERHMS_Incidents");
                    sql.AddSeparator();
                    sql.AddTable(JoinType.Inner, "ERHMS_Jobs", "ERHMS_Incidents", "IncidentId");
                    sql.AddSeparator();
                    sql.AddTableFromClause(JoinType.Inner, "ERHMS_JobTeams", "ERHMS_Jobs", "JobId");
                    sql.AddTable(JoinType.Inner, "ERHMS_Teams", "ERHMS_JobTeams", "TeamId");
                    sql.AddSeparator();
                    sql.AddTableFromClause(JoinType.Inner, "ERHMS_TeamResponders", "ERHMS_Teams", "TeamId");
                    foreach (string tableName in Context.Responders.TableNames)
                    {
                        sql.AddTableSelectClause(tableName);
                        sql.FromClauses.Add(string.Format("INNER JOIN {0} ON [ERHMS_TeamResponders].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName)));
                    }
                    sql.AddSeparator();
                    sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles", "ERHMS_TeamResponders", "IncidentRoleId");
                    sql.OtherClauses = clauses;
                    Func<Incident, Job, Team, Responder, IncidentRole, JobTicket> map = (incident, job, team, responder, incidentRole) =>
                    {
                        return Map(incident, job, team, responder, incidentRole, locations);
                    };
                    result1 = connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
                }
                IEnumerable<JobTicket> result2;
                {
                    SqlBuilder sql = new SqlBuilder();
                    sql.AddTable("ERHMS_Incidents");
                    sql.AddSeparator();
                    sql.AddTable(JoinType.Inner, "ERHMS_Jobs", "ERHMS_Incidents", "IncidentId");
                    sql.AddSeparator();
                    sql.AddTableFromClause(JoinType.Inner, "ERHMS_JobResponders", "ERHMS_Jobs", "JobId");
                    foreach (string tableName in Context.Responders.TableNames)
                    {
                        sql.AddTableSelectClause(tableName);
                        sql.FromClauses.Add(string.Format("INNER JOIN {0} ON [ERHMS_JobResponders].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName)));
                    }
                    sql.AddSeparator();
                    sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles", "ERHMS_JobResponders", "IncidentRoleId");
                    sql.OtherClauses = clauses;
                    Func<Incident, Job, Responder, IncidentRole, JobTicket> map = (incident, job, responder, incidentRole) =>
                    {
                        return Map(incident, job, null, responder, incidentRole, locations);
                    };
                    result2 = connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
                }
                return result1.Concat(result2);
            });
        }

        public override JobTicket SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<JobTicket> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Incidents].[IncidentId] = @IncidentId AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobTicket> SelectUndeletedByResponderId(string responderId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Incidents].[Deleted] = 0 AND {0}.[GlobalRecordId] = @ResponderId",
                Escape(Context.Responders.View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public override void Insert(JobTicket entity)
        {
            throw new NotSupportedException();
        }

        public override void Update(JobTicket entity)
        {
            throw new NotSupportedException();
        }

        public override void Delete(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public override void DeleteById(object id)
        {
            throw new NotSupportedException();
        }

        public override void Delete(JobTicket entity)
        {
            throw new NotSupportedException();
        }
    }
}
