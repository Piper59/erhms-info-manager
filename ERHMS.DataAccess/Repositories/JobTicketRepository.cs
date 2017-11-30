using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
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

        public DataContext Context { get; private set; }

        public JobTicketRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        public override int Count(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        private SqlBuilder GetTeamSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_Incidents");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.IncidentId", "ERHMS_Incidents.IncidentId");
            sql.AddSeparator();
            sql.AddTableFromClause(JoinType.Inner, "ERHMS_JobTeams.JobId", "ERHMS_Jobs.JobId");
            sql.AddTable(JoinType.Inner, "ERHMS_Teams.TeamId", "ERHMS_JobTeams.TeamId");
            sql.AddSeparator();
            sql.AddTableFromClause(JoinType.Inner, "ERHMS_TeamResponders.TeamId", "ERHMS_Teams.TeamId");
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_TeamResponders", "ResponderId");
            }
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles.IncidentRoleId", "ERHMS_TeamResponders.IncidentRoleId");
            return sql;
        }

        private SqlBuilder GetNonTeamSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_Incidents");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Jobs.IncidentId", "ERHMS_Incidents.IncidentId");
            sql.AddSeparator();
            sql.AddTableFromClause(JoinType.Inner, "ERHMS_JobResponders.JobId", "ERHMS_Jobs.JobId");
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_JobResponders", "ResponderId");
            }
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles.IncidentRoleId", "ERHMS_JobResponders.IncidentRoleId");
            return sql;
        }

        private JobTicket Map(
            Incident incident,
            Job job,
            Team team,
            Responder responder,
            IncidentRole incidentRole,
            ILookup<string, Location> locations)
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

        public override IEnumerable<JobTicket> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                ILookup<string, Location> locations = Context.JobLocations.Select().ToLookup(
                    jobLocation => jobLocation.JobId,
                    jobLocation => jobLocation.Location,
                    StringComparer.OrdinalIgnoreCase);
                IEnumerable<JobTicket> teamJobTickets;
                {
                    SqlBuilder sql = GetTeamSqlBuilder();
                    sql.OtherClauses = clauses;
                    Func<Incident, Job, Team, Responder, IncidentRole, JobTicket> map = (incident, job, team, responder, incidentRole) =>
                    {
                        return Map(incident, job, team, responder, incidentRole, locations);
                    };
                    teamJobTickets = connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
                }
                IEnumerable<JobTicket> nonTeamJobTickets;
                {
                    SqlBuilder sql = GetNonTeamSqlBuilder();
                    sql.OtherClauses = clauses;
                    Func<Incident, Job, Responder, IncidentRole, JobTicket> map = (incident, job, responder, incidentRole) =>
                    {
                        return Map(incident, job, null, responder, incidentRole, locations);
                    };
                    nonTeamJobTickets = connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
                }
                return teamJobTickets.Concat(nonTeamJobTickets);
            });
        }

        public override JobTicket SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<JobTicket> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Incidents].[IncidentId] = @IncidentId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<JobTicket> SelectUndeletedByIncidentIdAndDateRange(string incidentId, DateTime? startDate, DateTime? endDate)
        {
            ICollection<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            conditions.Add(string.Format(
                "[ERHMS_Incidents].[IncidentId] = @IncidentId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS)));
            parameters.Add("@IncidentId", incidentId);
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

        public IEnumerable<JobTicket> SelectUndeletedByResponderId(string responderId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Incidents].[Deleted] = 0 AND {0}.{1} = @ResponderId",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.GLOBAL_RECORD_ID));
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
