using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using View = ERHMS.Domain.View;

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
            typeMap.Get(nameof(WebSurvey.View)).SetComputed();
            SqlMapper.SetTypeMap(typeof(WebSurvey), typeMap);
        }

        public new DataContext Context { get; private set; }

        public WebSurveyRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override IEnumerable<WebSurvey> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string format = @"
                    SELECT [ERHMS_WebSurveys].*, NULL AS [Separator], [metaViews].*
                    FROM [ERHMS_WebSurveys]
                    INNER JOIN [metaViews] ON [ERHMS_WebSurveys].[ViewId] = [metaViews].[ViewId]
                    {0}";
                string sql = string.Format(format, clauses);
                Func<WebSurvey, View, WebSurvey> map = (webSurvey, view) =>
                {
                    webSurvey.New = false;
                    view.New = false;
                    webSurvey.View = view;
                    return webSurvey;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator");
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
