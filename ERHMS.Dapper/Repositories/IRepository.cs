using System.Collections.Generic;

namespace ERHMS.Dapper
{
    public interface IRepository<TEntity, TId>
    {
        int Count(string sql = null, object parameters = null);
        IEnumerable<TEntity> Select(string sql = null, object parameters = null);
        TEntity SelectById(TId id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(string sql = null, object parameters = null);
        void DeleteById(TId id);
        void Delete(TEntity entity);
    }
}
