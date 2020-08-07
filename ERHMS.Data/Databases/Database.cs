﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ERHMS.Data.Databases
{
    public abstract class Database : IDatabase
    {
        protected static IDbConnection Connect(IDbConnection connection)
        {
            connection = new LoggingConnection(connection, Log.Default);
            connection.Open();
            return connection;
        }

        public abstract DatabaseType Type { get; }
        protected abstract DbConnectionStringBuilder ConnectionStringBuilder { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;
        public abstract string Name { get; }

        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new DbConnectionStringBuilder
            {
                ConnectionString = ConnectionString
            };
        }

        public abstract bool Exists();
        protected abstract void CreateCore();

        public void Create()
        {
            Log.Default.Debug($"Creating database: {this}");
            CreateCore();
        }

        protected abstract DbConnection GetConnection();

        public IDbConnection Connect()
        {
            return Connect(GetConnection());
        }

        public virtual string Quote(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public IEnumerable<string> GetTableNames()
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();
                DataTable table = connection.GetSchema("Tables");
                foreach (DataRow row in table.Rows)
                {
                    yield return row.Field<string>("TABLE_NAME");
                }
            }
        }

        public bool TableExists(string tableName)
        {
            return GetTableNames().Contains(tableName, StringComparer.OrdinalIgnoreCase);
        }
    }
}