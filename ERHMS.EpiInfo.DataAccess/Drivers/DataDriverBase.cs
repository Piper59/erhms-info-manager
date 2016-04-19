using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class DataDriverBase : IDataDriver
    {
        private static readonly Regex Comment = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);

        private DbProviderFactory factory;

        public DataProvider Provider { get; private set; }
        public string ConnectionString { get; private set; }

        protected DataDriverBase(DataProvider provider, DbConnectionStringBuilder builder)
        {
            Log.Current.DebugFormat("Creating data driver: {0}, {1}", provider.ToInvariantName(), builder.ToSafeString());
            factory = DbProviderFactories.GetFactory(provider.ToInvariantName());
            Provider = provider;
            ConnectionString = builder.ConnectionString;
        }

        public string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public abstract string GetParameterName(int index);

        private DbConnection GetConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
            return connection;
        }

        public DataTransaction BeginTransaction()
        {
            DbConnection connection = GetConnection();
            DataTransaction transaction = new DataTransaction(connection);
            transaction.Completed += (sender, e) =>
            {
                connection.Dispose();
            };
            return transaction;
        }

        public DataTable GetSchema(string sql)
        {
            Log.Current.DebugFormat("Getting schema: {0}", sql);
            using (DbConnection connection = GetConnection())
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

        public DataTable ExecuteQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters)
        {
            Log.Current.DebugFormat("Executing SQL: {0}", sql);
            using (DbCommand command = transaction.CreateCommand())
            {
                command.CommandText = sql;
                parameters.AddToCommand(command);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public DataTable ExecuteQuery(DataTransaction transaction, string sql, params DataParameter[] parameters)
        {
            return ExecuteQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
        }

        public DataTable ExecuteQuery(string sql, IEnumerable<DataParameter> parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                DataTable result = ExecuteQuery(transaction, sql, parameters);
                transaction.Commit();
                return result;
            }
        }

        public DataTable ExecuteQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                DataTable result = ExecuteQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
                transaction.Commit();
                return result;
            }
        }

        public int ExecuteNonQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters)
        {
            Log.Current.DebugFormat("Executing SQL: {0}", sql);
            using (DbCommand command = transaction.CreateCommand())
            {
                command.CommandText = sql;
                parameters.AddToCommand(command);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(DataTransaction transaction, string sql, params DataParameter[] parameters)
        {
            return ExecuteNonQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
        }

        public int ExecuteNonQuery(string sql, IEnumerable<DataParameter> parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                int result = ExecuteNonQuery(transaction, sql, parameters);
                transaction.Commit();
                return result;
            }
        }

        public int ExecuteNonQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                int result = ExecuteNonQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
                transaction.Commit();
                return result;
            }
        }

        public void ExecuteScript(string script)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                foreach (string sql in Comment.Replace(script, "").Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        continue;
                    }
                    ExecuteNonQuery(transaction, sql);
                }
                transaction.Commit();
            }
        }

        public void Dispose() { }
    }
}
