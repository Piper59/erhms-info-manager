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
    public class TeamResponderRepository : EntityRepository<TeamResponder>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(TeamResponder))
            {
                TableName = "ERHMS_TeamResponders"
            };
            typeMap.Get(nameof(TeamResponder.TeamResponderId)).SetId();
            typeMap.Get(nameof(TeamResponder.New)).SetComputed();
            typeMap.Get(nameof(TeamResponder.Team)).SetComputed();
            typeMap.Get(nameof(TeamResponder.Responder)).SetComputed();
            typeMap.Get(nameof(TeamResponder.IncidentRole)).SetComputed();
            SqlMapper.SetTypeMap(typeof(TeamResponder), typeMap);
        }

        public new DataContext Context { get; private set; }

        public TeamResponderRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override IEnumerable<TeamResponder> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_TeamResponders");
                sql.AddSeparator();
                sql.AddTable(JoinType.Inner, "ERHMS_Teams", "ERHMS_TeamResponders", "TeamId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_TeamIncidents");
                sql.FromClauses.Add("INNER JOIN [ERHMS_Incidents] AS [ERHMS_TeamIncidents] ON [ERHMS_Teams].[IncidentId] = [ERHMS_TeamIncidents].[IncidentId]");
                sql.AddSeparator();
                foreach (string tableName in Context.Responders.TableNames)
                {
                    sql.AddTableSelectClause(tableName);
                    sql.FromClauses.Add(string.Format("INNER JOIN {0} ON [ERHMS_TeamResponders].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName)));
                }
                sql.AddSeparator();
                sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles", "ERHMS_TeamResponders", "IncidentRoleId");
                sql.AddSeparator();
                sql.AddTableSelectClause("ERHMS_IncidentRoleIncidents");
                sql.FromClauses.Add("LEFT OUTER JOIN [ERHMS_Incidents] AS [ERHMS_IncidentRoleIncidents] ON [ERHMS_IncidentRoles].[IncidentId] = [ERHMS_IncidentRoleIncidents].[IncidentId]");
                sql.OtherClauses = clauses;
                Func<TeamResponder, Team, Incident, Responder, IncidentRole, Incident, TeamResponder> map = (teamResponder, team, teamIncident, responder, incidentRole, incidentRoleIncident) =>
                {
                    teamResponder.Team = team;
                    team.Incident = teamIncident;
                    teamResponder.Responder = responder;
                    if (incidentRole.IncidentRoleId != null)
                    {
                        teamResponder.IncidentRole = incidentRole;
                        incidentRole.Incident = incidentRoleIncident;
                    }
                    return teamResponder;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<TeamResponder> SelectUndeleted()
        {
            string format = @"
                WHERE [ERHMS_TeamIncidents].[Deleted] = 0
                AND ([ERHMS_IncidentRoleIncidents].[Deleted] IS NULL OR [ERHMS_IncidentRoleIncidents].[Deleted] = 0)
                AND {0}.[RECSTATUS] <> 0";
            return Select(string.Format(format, Escape(Context.Responders.View.TableName)));
        }

        public override TeamResponder SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_TeamResponders].[TeamResponderId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        private IEnumerable<TeamResponder> SelectByIncidentIdInternal(string clauses, string incidentId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentIncidentId", incidentId);
            parameters.Add("@IncidentRolesIncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<TeamResponder> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Teams].[IncidentId] = @IncidentIncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentRolesIncidentId";
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        public IEnumerable<TeamResponder> SelectUndeletedByIncidentId(string incidentId)
        {
            string format = @"
                WHERE ([ERHMS_Teams].[IncidentId] = @IncidentIncidentId OR [ERHMS_IncidentRoles].[IncidentId] = @IncidentRolesIncidentId)
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(Context.Responders.View.TableName));
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        private IEnumerable<TeamResponder> SelectByTeamIdInternal(string clauses, string teamId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<TeamResponder> SelectByTeamId(string teamId)
        {
            return SelectByTeamIdInternal("WHERE [ERHMS_TeamResponders].[TeamId] = @TeamId", teamId);
        }

        public IEnumerable<TeamResponder> SelectUndeletedByTeamId(string teamId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_TeamResponders].[TeamId] = @TeamId AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            return SelectByTeamIdInternal(clauses, teamId);
        }

        public IEnumerable<TeamResponder> SelectUndeletedByResponderId(string responderId)
        {
            string clauses = @"
                WHERE [ERHMS_TeamResponders].[ResponderId] = @ResponderId
                AND [ERHMS_TeamIncidents].[Deleted] = 0
                AND ([ERHMS_IncidentRoleIncidents].[Deleted] IS NULL OR [ERHMS_IncidentRoleIncidents].[Deleted] = 0)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void DeleteByTeamId(string teamId)
        {
            string clauses = "WHERE [TeamId] = @TeamId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            Delete(clauses, parameters);
        }
    }
}
