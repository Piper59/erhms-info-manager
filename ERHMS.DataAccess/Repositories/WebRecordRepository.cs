using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class WebRecordRepository : EntityRepository<WebRecord>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(WebRecord))
            {
                TableName = "ERHMS_WebRecords"
            };
            typeMap.Get(nameof(WebRecord.New)).SetComputed();
            typeMap.Get(nameof(WebRecord.Id)).SetComputed();
            typeMap.Get(nameof(WebRecord.Guid)).SetComputed();
            typeMap.Get(nameof(WebRecord.WebRecordId)).SetId();
            typeMap.Get(nameof(WebRecord.Responder)).SetComputed();
            SqlMapper.SetTypeMap(typeof(WebRecord), typeMap);
        }

        public DataContext Context { get; private set; }

        public WebRecordRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_WebRecords");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_WebRecords", "ResponderId");
            }
            return sql;
        }

        private WebRecord Map(WebRecord webRecord, Responder responder)
        {
            webRecord.Responder = responder;
            return webRecord;
        }

        public override IEnumerable<WebRecord> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<WebRecord, Responder, WebRecord>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override WebRecord SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_WebRecords].[WebRecordId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<WebRecord> SelectByWebSurveyId(string webSurveyId)
        {
            string clauses = "WHERE [ERHMS_WebRecords].[WebSurveyId] = @WebSurveyId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@WebSurveyId", webSurveyId);
            return Select(clauses, parameters);
        }
    }
}
