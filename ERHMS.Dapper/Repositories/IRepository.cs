using System.Collections.Generic;

namespace ERHMS.Dapper
{
    public interface IRepository<TEntity>
    {
        int Count(string clauses = null, object parameters = null);
        IEnumerable<TEntity> Select(string clauses = null, object parameters = null);
        TEntity SelectById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Save(TEntity entity);
        void Delete(string clauses = null, object parameters = null);
        void DeleteById(object id);
        void Delete(TEntity entity);
    }
}
