using System.Collections.Generic;

namespace ERHMS.Data
{
    public interface IRepository<TEntity>
    {
        string Quote(string identifier);
        int Count(string clauses, object parameters);
        IEnumerable<TEntity> Select(string clauses, object parameters);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }

    public static class IRepositoryExtensions
    {
        public static int Count<TEntity>(this IRepository<TEntity> @this)
        {
            return @this.Count(null, null);
        }

        public static int Count<TEntity>(this IRepository<TEntity> @this, string clauses)
        {
            return @this.Count(clauses, null);
        }

        public static IEnumerable<TEntity> Select<TEntity>(this IRepository<TEntity> @this)
        {
            return @this.Select(null, null);
        }

        public static IEnumerable<TEntity> Select<TEntity>(this IRepository<TEntity> @this, string clauses)
        {
            return @this.Select(clauses, null);
        }
    }
}
