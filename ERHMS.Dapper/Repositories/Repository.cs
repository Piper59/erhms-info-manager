using Dapper;
using System.Collections.Generic;

namespace ERHMS.Dapper
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    {
        protected TypeMap TypeMap { get; private set; }
        public IDatabase Database { get; private set; }

        public Repository(IDatabase database)
        {
            TypeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(TEntity));
            Database = database;
        }

        public virtual int Count(string sql = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                sql = string.Format("SELECT COUNT(*) FROM {0} {1}", IDbConnectionExtensions.Escape(TypeMap.TableName), sql);
                return connection.ExecuteScalar<int>(sql, parameters, transaction);
            });
        }

        public virtual IEnumerable<TEntity> Select(string sql = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                return connection.Select<TEntity>(sql, parameters, transaction);
            });
        }

        public virtual TEntity SelectById(TId id)
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

        public virtual void Delete(string sql = null, object parameters = null)
        {
            Database.Invoke((connection, transaction) =>
            {
                connection.Delete<TEntity>(sql, parameters, transaction);
            });
        }

        public virtual void DeleteById(TId id)
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
