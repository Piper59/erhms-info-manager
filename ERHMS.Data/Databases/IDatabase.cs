﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data.Databases
{
    public interface IDatabase
    {
        DatabaseType Type { get; }
        string ConnectionString { get; }
        string Name { get; }

        DbConnectionStringBuilder GetConnectionStringBuilder();
        bool Exists();
        void Create();
        IDbConnection Connect();
        string Quote(string identifier);
        IEnumerable<string> GetTableNames();
        bool TableExists(string tableName);
    }
}
