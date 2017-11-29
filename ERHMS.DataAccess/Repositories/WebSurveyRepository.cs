using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class WebSurveyRepository : EntityRepository<WebSurvey>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(WebSurvey))
            {
                TableName = "ERHMS_WebSurveys"
            };
            typeMap.Get(nameof(WebSurvey.WebSurveyId)).SetId();
            typeMap.Get(nameof(WebSurvey.New)).SetComputed();
            typeMap.Get(nameof(WebSurvey.View)).SetComputed();
            SqlMapper.SetTypeMap(typeof(WebSurvey), typeMap);
        }

        public WebSurveyRepository(DataContext context)
            : base(context.Database) { }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_WebSurveys");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "metaViews.ViewId", "ERHMS_WebSurveys.ViewId");
            sql.SelectClauses.Add(ViewRepository.HasResponderIdFieldSql);
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_ViewLinks.ViewId", "metaViews.ViewId");
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents.IncidentId", "ERHMS_ViewLinks.IncidentId");
            return sql;
        }

        private WebSurvey Map(WebSurvey webSurvey, View view, ViewLink viewLink, Incident incident)
        {
            webSurvey.View = view;
            if (viewLink.ViewLinkId != null)
            {
                view.Link = viewLink;
                viewLink.Incident = incident;
            }
            return webSurvey;
        }

        public override IEnumerable<WebSurvey> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<WebSurvey, View, ViewLink, Incident, WebSurvey>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override WebSurvey SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_WebSurveys].[WebSurveyId] = @Id";
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
