using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ERHMS.Data
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
    {
        public IDatabase Database { get; }

        protected Repository(IDatabase database)
        {
            Database = database;
        }

        public string Quote(string identifier)
        {
            return Database.Quote(identifier);
        }

        protected abstract string GetTableSource();

        protected string GetSelectStatement(string selectList, string clauses)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"SELECT {selectList} FROM {GetTableSource()}");
            if (clauses != null)
            {
                sql.Append($" {clauses}");
            }
            sql.Append(";");
            return sql.ToString();
        }

        public virtual int Count(string clauses, object parameters)
        {
            string sql = GetSelectStatement("COUNT(*)", clauses);
            using (IDbConnection connection = Database.Connect())
            {
                return connection.ExecuteScalar<int>(sql, parameters);
            }
        }

        public virtual IEnumerable<TEntity> Select(string clauses, object parameters)
        {
            string sql = GetSelectStatement("*", clauses);
            using (IDbConnection connection = Database.Connect())
            {
                return connection.Query<TEntity>(sql, parameters);
            }
        }

        public virtual void Insert(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public virtual void Update(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public virtual void Delete(TEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
