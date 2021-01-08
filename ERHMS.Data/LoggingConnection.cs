using ERHMS.Common.Logging;
using System.Data;

namespace ERHMS.Data
{
    public class LoggingConnection : IDbConnection
    {
        public string DatabaseId { get; }
        public IDbConnection UnderlyingConnection { get; }

        public string ConnectionString
        {
            get { return UnderlyingConnection.ConnectionString; }
            set { UnderlyingConnection.ConnectionString = value; }
        }

        public int ConnectionTimeout => UnderlyingConnection.ConnectionTimeout;
        public string Database => UnderlyingConnection.Database;
        public ConnectionState State => UnderlyingConnection.State;

        public LoggingConnection(string databaseId, IDbConnection connection)
        {
            DatabaseId = databaseId;
            UnderlyingConnection = connection;
        }

        public IDbTransaction BeginTransaction()
        {
            return UnderlyingConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return UnderlyingConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            UnderlyingConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            UnderlyingConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return new LoggingCommand(UnderlyingConnection.CreateCommand());
        }

        public void Dispose()
        {
            UnderlyingConnection.Dispose();
        }

        public void Open()
        {
            Log.Instance.Debug($"Opening database connection: {DatabaseId}");
            UnderlyingConnection.Open();
        }
    }
}
