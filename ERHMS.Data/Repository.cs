using Dapper;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.Data
{
    public abstract class Repository<TEntity>
    {
        public IDatabase Database { get; }

        protected Repository(IDatabase database)
        {
            Database = database;
        }

        protected string Quote(string identifier)
        {
            return Database.Quote(identifier);
        }

        protected TResult ExecuteScalar<TResult>(QueryInfo query, string selectList)
        {
            string sql = query.GetSql(selectList);
            using (IDbConnection connection = Database.Connect())
            {
                return connection.ExecuteScalar<TResult>(sql, query.Parameters);
            }
        }

        protected IEnumerable<TEntity> Query(QueryInfo query, string selectList)
        {
            string sql = query.GetSql(selectList);
            using (IDbConnection connection = Database.Connect())
            {
                return connection.Query<TEntity>(sql, query.Parameters);
            }
        }
    }
}
