using ERHMS.Data.Databases;
using System.Collections.Generic;

namespace ERHMS.Data.Repositories
{
    public interface IRepository<TEntity>
    {
        IDatabase Database { get; }

        int Count(string clauses, object parameters);
        IEnumerable<TEntity> Select(string clauses, object parameters);
    }
}
