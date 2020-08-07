using log4net;
using System.Data;

namespace ERHMS.Data
{
    public class LoggingConnection : IDbConnection
    {
        public ILog Log { get; }

        private IDbConnection @base;

        public string ConnectionString
        {
            get { return @base.ConnectionString; }
            set { @base.ConnectionString = value; }
        }

        public int ConnectionTimeout => @base.ConnectionTimeout;
        public string Database => @base.Database;
        public ConnectionState State => @base.State;

        public LoggingConnection(IDbConnection @base, ILog log)
        {
            this.@base = @base;
            Log = log;
        }

        public IDbTransaction BeginTransaction() => @base.BeginTransaction();
        public IDbTransaction BeginTransaction(IsolationLevel il) => @base.BeginTransaction(il);
        public void ChangeDatabase(string databaseName) => @base.ChangeDatabase(databaseName);
        public void Close() => @base.Close();
        public void Dispose() => @base.Dispose();
        public void Open() => @base.Open();

        public IDbCommand CreateCommand()
        {
            return new LoggingCommand(@base.CreateCommand(), Log);
        }
    }
}
