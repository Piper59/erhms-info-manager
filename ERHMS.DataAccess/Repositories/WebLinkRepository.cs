using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class WebLinkRepository : EntityRepository<WebLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(WebLink))
            {
                TableName = "ERHMS_WebLinks"
            };
            typeMap.Get(nameof(WebLink.New)).SetComputed();
            typeMap.Get(nameof(WebLink.Id)).SetComputed();
            typeMap.Get(nameof(WebLink.Guid)).SetComputed();
            typeMap.Get(nameof(WebLink.WebLinkId)).SetId();
            typeMap.Get(nameof(WebLink.Responder)).SetComputed();
            SqlMapper.SetTypeMap(typeof(WebLink), typeMap);
        }

        public DataContext Context { get; private set; }

        public WebLinkRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_WebLinks");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_WebLinks", "ResponderId");
            }
            return sql;
        }

        private WebLink Map(WebLink webLink, Responder responder)
        {
            webLink.Responder = responder;
            return webLink;
        }

        public override IEnumerable<WebLink> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<WebLink, Responder, WebLink>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override WebLink SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_WebLinks].[WebLinkId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<WebLink> SelectBySurveyId(string surveyId)
        {
            string clauses = "WHERE [ERHMS_WebLinks].[SurveyId] = @SurveyId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SurveyId", surveyId);
            return Select(clauses, parameters);
        }
    }
}
