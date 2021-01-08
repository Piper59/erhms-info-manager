using ERHMS.Data.Databases;
using System.Collections.Generic;

namespace ERHMS.Data.Repositories
{
    public interface IRepository<T>
    {
        IDatabase Database { get; }

        int Count(string clauses, object parameters);
        IEnumerable<T> Select(string clauses, object parameters);
    }
}
