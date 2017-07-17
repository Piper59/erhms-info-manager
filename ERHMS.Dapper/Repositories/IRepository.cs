using System.Collections.Generic;

namespace ERHMS.Dapper
{
    public interface IRepository<TEntity>
    {
        int Count(string sql = null, object parameters = null);
        IEnumerable<TEntity> Select(string sql = null, object parameters = null);
        TEntity SelectById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(string sql = null, object parameters = null);
        void DeleteById(object id);
        void Delete(TEntity entity);
    }
}
