using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            typeMap.Get(nameof(Assignment.AssignmentId)).SetId();
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
                ICollection<string> tableNames = Context.Responders.TableNames.ToList();
                StringBuilder format = new StringBuilder();
                format.AppendFormat(
                    "SELECT [ERHMS_Assignments].*, NULL AS [Separator1], [metaViews].*, NULL AS [Separator2], {0}",
                    string.Join(", ", tableNames.Select(tableName => string.Format("{0}.*", Escape(tableName)))));
                format.AppendFormat(" FROM {0}", new string('(', tableNames.Count));
                format.Append("[ERHMS_Assignments] INNER JOIN [metaViews] ON [ERHMS_Assignments].[ViewId] = [metaViews].[ViewId]");
                foreach (string tableName in tableNames)
                {
                    format.AppendFormat(") INNER JOIN {0} ON [ERHMS_Assignments].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName));
                }
                format.Append(" {0}");
                string sql = string.Format(format.ToString(), clauses);
                Func<Assignment, View, Responder, Assignment> map = (assignment, view, responder) =>
                {
                    assignment.New = false;
                    view.New = false;
                    responder.New = false;
                    assignment.View = view;
                    assignment.Responder = responder;
                    return assignment;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator1, Separator2");
            });
        }

        public override Assignment SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Assignments].[AssignmentId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
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
