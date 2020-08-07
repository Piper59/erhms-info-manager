using log4net;
using System.Data;

namespace ERHMS.Data
{
    public class LoggingCommand : IDbCommand
    {
        public ILog Log { get; }

        private IDbCommand @base;

        public string CommandText
        {
            get { return @base.CommandText; }
            set { @base.CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return @base.CommandTimeout; }
            set { @base.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return @base.CommandType; }
            set { @base.CommandType = value; }
        }

        public IDbConnection Connection
        {
            get { return @base.Connection; }
            set { @base.Connection = value; }
        }

        public IDataParameterCollection Parameters
        {
            get { return @base.Parameters; }
        }

        public IDbTransaction Transaction
        {
            get { return @base.Transaction; }
            set { @base.Transaction = value; }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get { return @base.UpdatedRowSource; }
            set { @base.UpdatedRowSource = value; }
        }

        public LoggingCommand(IDbCommand @base, ILog log)
        {
            this.@base = @base;
            Log = log;
        }

        public void Cancel() => @base.Cancel();
        public IDbDataParameter CreateParameter() => @base.CreateParameter();
        public void Dispose() => @base.Dispose();
        public void Prepare() => @base.Prepare();

        private void OnExecuting()
        {
            Log.Debug($"Executing SQL: {CommandText.Trim()}");
        }

        public int ExecuteNonQuery()
        {
            OnExecuting();
            return @base.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            OnExecuting();
            return @base.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            OnExecuting();
            return @base.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            OnExecuting();
            return @base.ExecuteScalar();
        }
    }
}
