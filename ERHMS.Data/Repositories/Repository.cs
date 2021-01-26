using Dapper;
using ERHMS.Data.Databases;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ERHMS.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
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

        protected abstract string GetFromClause();

        protected string GetSelectQuery(string selectList, string clauses)
        {
            StringBuilder query = new StringBuilder();
            query.Append($"SELECT {selectList} {GetFromClause()}");
            if (clauses != null)
            {
                query.Append($" {clauses}");
            }
            query.Append(";");
            return query.ToString();
        }

        public virtual int Count(string clauses = null, object parameters = null)
        {
            using (IDbConnection connection = Database.Connect())
            {
                string sql = GetSelectQuery("COUNT(*)", clauses);
                return connection.ExecuteScalar<int>(sql, parameters);
            }
        }

        public virtual IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            using (IDbConnection connection = Database.Connect())
            {
                string sql = GetSelectQuery("*", clauses);
                return connection.Query<TEntity>(sql, parameters);
            }
        }
    }
}
