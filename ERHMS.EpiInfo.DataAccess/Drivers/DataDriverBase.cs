using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class DataDriverBase : IDataDriver
    {
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
            Log.Current.DebugFormat("Opening data driver: {0}, {1}", provider.ToInvariantName(), builder.GetCensoredConnectionString());
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
                DataTable table = ExecuteQuery(transaction, sql, parameters);
                transaction.Commit();
                return table;
            }
        }

        public DataTable ExecuteQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                DataTable table = ExecuteQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
                transaction.Commit();
                return table;
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
                int rowCount = ExecuteNonQuery(transaction, sql, parameters);
                transaction.Commit();
                return rowCount;
            }
        }

        public int ExecuteNonQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                int rowCount = ExecuteNonQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
                transaction.Commit();
                return rowCount;
            }
        }

        public void ExecuteScript(string script)
        {
            Regex comment = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);
            IEnumerable<string> sqls = comment.Replace(script, "").Split(';');
            using (DataTransaction transaction = BeginTransaction())
            {
                foreach (string sql in sqls)
                {
                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        continue;
                    }
                    ExecuteNonQuery(transaction, sql.Trim());
                }
                transaction.Commit();
            }
        }
    }
}
