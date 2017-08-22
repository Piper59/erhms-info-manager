using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
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
            : base(context) { }

        public override IEnumerable<WebSurvey> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_WebSurveys");
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.Inner, "metaViews", "ViewId", "ERHMS_WebSurveys"));
                sql.SelectClauses.Add(ViewRepository.HasResponderIdFieldSql);
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.LeftOuter, "ERHMS_ViewLinks", "ViewId", "metaViews"));
                sql.AddSeparator();
                sql.AddTable(new JoinInfo(JoinType.LeftOuter, "ERHMS_Incidents", "IncidentId", "ERHMS_ViewLinks"));
                sql.OtherClauses = clauses;
                Func<WebSurvey, View, ViewLink, Incident, WebSurvey> map = (webSurvey, view, viewLink, incident) =>
                {
                    webSurvey.View = view;
                    if (viewLink.ViewLinkId != null)
                    {
                        view.Link = viewLink;
                        viewLink.Incident = incident;
                    }
                    return webSurvey;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override WebSurvey SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_WebSurveys].[WebSurveyId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public WebSurvey SelectByViewId(int viewId)
        {
            string clauses = "WHERE [ERHMS_WebSurveys].[ViewId] = @ViewId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ViewId", viewId);
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
