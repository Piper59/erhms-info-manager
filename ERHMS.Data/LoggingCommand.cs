using log4net;
using System.Data;

namespace ERHMS.Data
{
    public class LoggingCommand : IDbCommand
    {
        private IDbCommand @base;
        private ILog log;

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
            this.log = log;
        }

        public void Cancel()
        {
            @base.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return @base.CreateParameter();
        }

        public void Dispose()
        {
            @base.Dispose();
        }

        private void OnExecuting()
        {
            log.Debug($"Executing SQL: {CommandText.Trim()}");
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

        public void Prepare()
        {
            @base.Prepare();
        }
    }
}
