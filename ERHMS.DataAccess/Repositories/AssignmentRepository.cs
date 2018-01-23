using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;
using View = ERHMS.Domain.View;

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
            typeMap.Get(nameof(Assignment.New)).SetComputed();
            typeMap.Get(nameof(Assignment.Id)).SetComputed();
            typeMap.Get(nameof(Assignment.Guid)).SetComputed();
            typeMap.Get(nameof(Assignment.AssignmentId)).SetId();
            typeMap.Get(nameof(Assignment.View)).SetComputed();
            typeMap.Get(nameof(Assignment.Responder)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Assignment), typeMap);
        }

        public DataContext Context { get; private set; }

        public AssignmentRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_Assignments");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "metaViews.ViewId", "ERHMS_Assignments.ViewId");
            sql.SelectClauses.Add(ViewRepository.HasResponderIdFieldSql);
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_ViewLinks.ViewId", "metaViews.ViewId");
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents.IncidentId", "ERHMS_ViewLinks.IncidentId");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_Assignments", "ResponderId");
            }
            return sql;
        }

        private Assignment Map(Assignment assignment, View view, ViewLink viewLink, Incident incident, Responder responder)
        {
            assignment.View = view;
            if (viewLink.ViewLinkId != null)
            {
                view.Link = viewLink;
                viewLink.Incident = incident;
            }
            assignment.Responder = responder;
            return assignment;
        }

        public override IEnumerable<Assignment> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<Assignment, View, ViewLink, Incident, Responder, Assignment>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<Assignment> SelectUndeleted()
        {
            string clauses = string.Format(
                "WHERE ([ERHMS_Incidents].[Deleted] IS NULL OR [ERHMS_Incidents].[Deleted] = 0) AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
            return Select(clauses);
        }

        public override Assignment SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Assignments].[AssignmentId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<Assignment> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_ViewLinks].[IncidentId] = @IncidentId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
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
