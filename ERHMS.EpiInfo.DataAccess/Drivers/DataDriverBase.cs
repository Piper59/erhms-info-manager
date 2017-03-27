using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class DataDriverBase : IDataDriver
    {
        private static readonly Regex CommentPattern = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);

        private DbProviderFactory factory;

        public DataProvider Provider { get; private set; }
        public DbConnectionStringBuilder Builder { get; private set; }
        public string DatabaseName { get; private set; }

        public string ConnectionString
        {
            get { return Builder.ConnectionString; }
        }

        protected DataDriverBase(DataProvider provider, DbConnectionStringBuilder builder, string databaseName)
        {
            Log.Logger.DebugFormat("Opening data driver: {0}, {1}", provider.ToInvariantName(), builder.GetCensoredConnectionString());
            factory = DbProviderFactories.GetFactory(provider.ToInvariantName());
            Provider = provider;
            Builder = builder;
            DatabaseName = databaseName;
        }

        public string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public abstract string GetParameterName(int index);
        public abstract bool DatabaseExists();
        public abstract void CreateDatabase();

        private DbConnection OpenConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
            return connection;
        }

        public DataTransaction BeginTransaction()
        {
            DbConnection connection = OpenConnection();
            DataTransaction transaction = new DataTransaction(connection);
            transaction.Completed += (sender, e) =>
            {
                connection.Dispose();
            };
            return transaction;
        }

        public DataTable GetSchema(string sql)
        {
            Log.Logger.DebugFormat("Getting schema: {0}", sql);
            using (DbConnection connection = OpenConnection())
            using (DbDataAdapter adapter = factory.CreateDataAdapter())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                adapter.SelectCommand = command;
                DataTable schema = new DataTable();
                adapter.FillSchema(schema, SchemaType.Source);
                return schema;
            }
        }

        private T ExecuteScalar<T>(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters)
        {
            Log.Logger.DebugFormat("Executing SQL: {0}", sql);
            using (DbCommand command = transaction.CreateCommand())
            {
                command.CommandText = sql;
                foreach (DataParameter parameter in parameters)
                {
                    parameter.AddToCommand(command);
                }
                return (T)command.ExecuteScalar();
            }
        }

        private T ExecuteScalar<T>(string sql, IEnumerable<DataParameter> parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                T value = ExecuteScalar<T>(transaction, sql, parameters);
                transaction.Commit();
                return value;
            }
        }

        public T ExecuteScalar<T>(DataQuery query)
        {
            if (query.Transaction == null)
            {
                return ExecuteScalar<T>(query.Sql, query.Parameters);
            }
            else
            {
                return ExecuteScalar<T>(query.Transaction, query.Sql, query.Parameters);
            }
        }

        public T ExecuteScalar<T>(string sql)
        {
            return ExecuteScalar<T>(sql, Enumerable.Empty<DataParameter>());
        }

        private DataTable ExecuteQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters)
        {
            Log.Logger.DebugFormat("Executing SQL: {0}", sql);
            using (DbCommand command = transaction.CreateCommand())
            {
                command.CommandText = sql;
                foreach (DataParameter parameter in parameters)
                {
                    parameter.AddToCommand(command);
                }
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        private DataTable ExecuteQuery(string sql, IEnumerable<DataParameter> parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                DataTable table = ExecuteQuery(transaction, sql, parameters);
                transaction.Commit();
                return table;
            }
        }

        public DataTable ExecuteQuery(DataQuery query)
        {
            if (query.Transaction == null)
            {
                return ExecuteQuery(query.Sql, query.Parameters);
            }
            else
            {
                return ExecuteQuery(query.Transaction, query.Sql, query.Parameters);
            }
        }

        public DataTable ExecuteQuery(string sql)
        {
            return ExecuteQuery(sql, Enumerable.Empty<DataParameter>());
        }

        private void ExecuteNonQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters)
        {
            Log.Logger.DebugFormat("Executing SQL: {0}", sql);
            using (DbCommand command = transaction.CreateCommand())
            {
                command.CommandText = sql;
                foreach (DataParameter parameter in parameters)
                {
                    parameter.AddToCommand(command);
                }
                command.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQuery(string sql, IEnumerable<DataParameter> parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                ExecuteNonQuery(transaction, sql, parameters);
                transaction.Commit();
            }
        }

        public void ExecuteNonQuery(DataQuery query)
        {
            if (query.Transaction == null)
            {
                ExecuteNonQuery(query.Sql, query.Parameters);
            }
            else
            {
                ExecuteNonQuery(query.Transaction, query.Sql, query.Parameters);
            }
        }

        public void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, Enumerable.Empty<DataParameter>());
        }

        public void ExecuteScript(string script)
        {
            IEnumerable<string> sqls = CommentPattern.Replace(script, "").Split(';');
            using (DataTransaction transaction = BeginTransaction())
            {
                foreach (string sql in sqls)
                {
                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        continue;
                    }
                    ExecuteNonQuery(transaction, sql.Trim(), Enumerable.Empty<DataParameter>());
                }
                transaction.Commit();
            }
        }
    }
}
