using log4net.Core;
using System.Data;

namespace ERHMS.Data.Logging
{
    public class LoggingConnection : IDbConnection
    {
        public IDbConnection BaseConnection { get; }

        public string ConnectionString
        {
            get { return BaseConnection.ConnectionString; }
            set { BaseConnection.ConnectionString = value; }
        }

        public int ConnectionTimeout => BaseConnection.ConnectionTimeout;
        public string Database => BaseConnection.Database;
        public ConnectionState State => BaseConnection.State;
        public Level Level { get; set; } = Level.Debug;

        public LoggingConnection(IDbConnection connection)
        {
            BaseConnection = connection;
        }

        public IDbTransaction BeginTransaction()
        {
            return BaseConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return BaseConnection.BeginTransaction(isolationLevel);
        }

        public void ChangeDatabase(string databaseName)
        {
            BaseConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            BaseConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return new LoggingCommand(BaseConnection.CreateCommand(), Level);
        }

        public void Dispose()
        {
            BaseConnection.Dispose();
        }

        public void Open()
        {
            BaseConnection.Open();
        }
    }
}
