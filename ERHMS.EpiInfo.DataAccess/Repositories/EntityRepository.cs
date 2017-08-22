using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;

namespace ERHMS.EpiInfo.DataAccess
{
    public class EntityRepository<TEntity> : Repository<TEntity>
        where TEntity : Entity
    {
        public IDataContext Context { get; private set; }

        public Project Project
        {
            get { return Context.Project; }
        }

        public EntityRepository(IDataContext context)
            : base(context.Database)
        {
            Context = context;
        }

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
