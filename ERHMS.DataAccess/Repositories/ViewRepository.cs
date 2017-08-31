using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using View = ERHMS.Domain.View;

namespace ERHMS.DataAccess
{
    public class ViewRepository : LinkedEntityRepository<View, ViewLink>
    {
        internal const string HasResponderIdFieldSql = @"
            (
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

        protected override SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("metaViews");
            sql.SelectClauses.Add(HasResponderIdFieldSql);
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_ViewLinks", "metaViews", "ViewId");
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents", "ERHMS_ViewLinks", "IncidentId");
            return sql;
        }
    }
}
