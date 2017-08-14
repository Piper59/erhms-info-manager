using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using View = ERHMS.Domain.View;

namespace ERHMS.DataAccess
{
    public class ViewRepository : LinkedEntityRepository<View, ViewLink>
    {
        internal const string HasResponderIdFieldSql = @"(
                SELECT COUNT(*)
                FROM [metaFields]
                WHERE [metaFields].[ViewId] = [metaViews].[ViewId]
                AND [metaFields].[Name] = 'ResponderID'
            ) AS [HasResponderIdField]";

        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(View))
            {
                TableName = "metaViews"
            };
            typeMap.Get(nameof(View.ViewId)).SetId();
            typeMap.Get(nameof(View.New)).SetComputed();
            typeMap.Set(nameof(View.WebSurveyId), ColumnNames.CHECK_CODE_AFTER);
            typeMap.Get(nameof(View.HasResponderIdField)).SetComputed();
            typeMap.Get(nameof(View.Link)).SetComputed();
            typeMap.Get(nameof(View.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(View), typeMap);
        }

        public ViewRepository(DataContext context)
            : base(context) { }

        protected override SqlBuilder GetSelectSql()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable(TypeMap.TableName);
            sql.SelectClauses.Add(HasResponderIdFieldSql);
            sql.AddSeparator();
            sql.AddTable(new JoinInfo(JoinType.LeftOuter, "ERHMS_ViewLinks", "ViewId", "metaViews"));
            sql.AddSeparator();
            sql.AddTable(new JoinInfo(JoinType.LeftOuter, "ERHMS_Incidents", "IncidentId", "ERHMS_ViewLinks"));
            return sql;
        }
    }
}
