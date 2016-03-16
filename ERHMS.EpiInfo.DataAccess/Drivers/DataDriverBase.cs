using Epi;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class DataDriverBase : IDataDriver
    {
        private DbProviderFactory factory;
        private DbConnectionStringBuilder connectionStringBuilder;

        public Project Project { get; private set; }

        protected DataDriverBase(DataProvider provider, IDictionary<string, object> connectionProperties)
        {
            factory = DbProviderFactories.GetFactory(provider.GetInvariantName());
            connectionStringBuilder = factory.CreateConnectionStringBuilder();
            foreach (KeyValuePair<string, object> connectionProperty in connectionProperties)
            {
                connectionStringBuilder[connectionProperty.Key] = connectionProperty.Value;
            }
            Project = new InMemoryProject(provider.GetEpiInfoName(), connectionStringBuilder.ConnectionString);
        }

        public string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public abstract string GetParameterName(int index);

        private DbConnection GetConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionStringBuilder.ConnectionString;
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
                return ExecuteQuery(transaction, sql, parameters);
            }
        }

        public DataTable ExecuteQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                return ExecuteQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
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
                return ExecuteNonQuery(transaction, sql, parameters);
            }
        }

        public int ExecuteNonQuery(string sql, params DataParameter[] parameters)
        {
            using (DataTransaction transaction = BeginTransaction())
            {
                return ExecuteNonQuery(transaction, sql, (IEnumerable<DataParameter>)parameters);
            }
        }

        public void Dispose()
        {
            Project.Dispose();
        }
    }
}
