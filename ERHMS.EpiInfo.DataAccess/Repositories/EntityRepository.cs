using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;
using System.Collections.Generic;

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

        public override IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            foreach (TEntity entity in base.Select(clauses, parameters))
            {
                entity.New = false;
                yield return entity;
            }
        }

        public override TEntity SelectById(object id)
        {
            TEntity entity = base.SelectById(id);
            if (entity != null)
            {
                entity.New = false;
            }
            return entity;
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
