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
    public class AssignmentRepository : EntityRepository<Assignment>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Assignment))
            {
                TableName = "ERHMS_Assignments"
            };
            typeMap.Get(nameof(Assignment.AssignmentId)).SetId();
            typeMap.Get(nameof(Assignment.New)).SetComputed();
            typeMap.Get(nameof(Assignment.View)).SetComputed();
            typeMap.Get(nameof(Assignment.Responder)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Assignment), typeMap);
        }

        public new DataContext Context { get; private set; }

        public AssignmentRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override IEnumerable<Assignment> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_Assignments");
                sql.AddSeparator();
                sql.AddTable(JoinType.Inner, "metaViews", "ViewId", "ERHMS_Assignments");
                sql.SelectClauses.Add(ViewRepository.HasResponderIdFieldSql);
                sql.AddSeparator();
                sql.AddTable(JoinType.LeftOuter, "ERHMS_ViewLinks", "ViewId", "metaViews");
                sql.AddSeparator();
                sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents", "IncidentId", "ERHMS_ViewLinks");
                sql.AddSeparator();
                foreach (string tableName in Context.Responders.TableNames)
                {
                    sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_Assignments", "ResponderId");
                }
                sql.OtherClauses = clauses;
                Func<Assignment, Domain.View, ViewLink, Incident, Responder, Assignment> map = (assignment, view, viewLink, incident, responder) =>
                {
                    assignment.New = false;
                    view.New = false;
                    viewLink.New = false;
                    incident.New = false;
                    responder.New = false;
                    assignment.View = view;
                    if (viewLink.GetProperty(nameof(ViewLink.ViewId)) != null)
                    {
                        view.Link = viewLink;
                        viewLink.Incident = incident;
                    }
                    assignment.Responder = responder;
                    return assignment;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<Assignment> SelectUndeleted()
        {
            return Select(string.Format(
                "WHERE ([ERHMS_Incidents].[Deleted] IS NULL OR [ERHMS_Incidents].[Deleted] = 0) AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName)));
        }

        public override Assignment SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Assignments].[AssignmentId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        private IEnumerable<Assignment> SelectByIncidentIdInternal(string clauses, string incidentId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Assignment> SelectByIncidentId(string incidentId)
        {
            return SelectByIncidentIdInternal("WHERE [ERHMS_ViewLinks].[IncidentId] = @IncidentId", incidentId);
        }

        public IEnumerable<Assignment> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_ViewLinks].[IncidentId] = @IncidentId AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            return SelectByIncidentIdInternal(clauses, incidentId);
        }

        public void DeleteByViewId(int viewId)
        {
            string clauses = "WHERE [ViewId] = @ViewId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ViewId", viewId);
            Delete(clauses, parameters);
        }
    }
}
