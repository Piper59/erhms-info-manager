using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public static class IRepositoryExtensions
    {
        public static IEnumerable<TEntity> Refresh<TEntity>(this IRepository<TEntity> @this, IEnumerable<TEntity> entities)
            where TEntity : Entity
        {
            ISet<TEntity> set = new HashSet<TEntity>(entities);
            return @this.Select().Where(entity => set.Contains(entity));
        }

        public static TEntity Refresh<TEntity>(this IRepository<TEntity> @this, TEntity entity)
            where TEntity : Entity
        {
            return @this.SelectById(entity.Id);
        }
    }
}
