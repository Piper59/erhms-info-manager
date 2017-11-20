using Dapper;
using ERHMS.Utility;
using System;
using System.Collections.Generic;

namespace ERHMS.Dapper
{
    public class Repository<TEntity> : IRepository<TEntity>
    {
        protected static string Escape(string identifier)
        {
            return DbExtensions.Escape(identifier);
        }

        protected TypeMap TypeMap { get; private set; }
        public IDatabase Database { get; private set; }

        public Repository(IDatabase database)
        {
            TypeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(TEntity));
            Database = database;
        }

        public virtual int Count(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string sql = string.Format("SELECT COUNT(*) FROM {0} {1}", Escape(TypeMap.TableName), clauses);
                return connection.ExecuteScalar<int>(sql, parameters, transaction);
            });
        }

        public virtual IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                return connection.Select<TEntity>(clauses, parameters, transaction);
            });
        }

        public virtual TEntity SelectById(object id)
        {
            return Database.Invoke((connection, transaction) =>
            {
                return connection.SelectById<TEntity>(id, transaction);
            });
        }

        public virtual void Insert(TEntity entity)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.Insert(entity, transaction);
            });
        }

        public virtual void Update(TEntity entity)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.Update(entity, transaction);
            });
        }

        public virtual void Save(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public virtual void Delete(string clauses = null, object parameters = null)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.Delete<TEntity>(clauses, parameters, transaction);
            });
        }

        public virtual void DeleteById(object id)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.DeleteById<TEntity>(id, transaction);
            });
        }

        public virtual void Delete(TEntity entity)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.Delete(entity, transaction);
            });
        }
    }
}
