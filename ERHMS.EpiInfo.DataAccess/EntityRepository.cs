using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;

namespace ERHMS.EpiInfo.DataAccess
{
    public class EntityRepository<TEntity> : Repository<TEntity>
        where TEntity : Entity
    {
        public EntityRepository(IDatabase database)
            : base(database) { }

        public override void Insert(TEntity entity)
        {
            base.Insert(entity);
            entity.New = false;
        }

        public override void Save(TEntity entity)
        {
            if (entity.New)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }
    }
}
