using ERHMS.Utility;
using System.Data;

namespace ERHMS.Dapper
{
    public class LoggingCommand : IDbCommand
    {
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

        public LoggingCommand(IDbCommand @base)
        {
            this.@base = @base;
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

        private void PreExecute()
        {
            Log.Logger.DebugFormat("Executing SQL: {0}", CommandText.Trim());
        }

        public int ExecuteNonQuery()
        {
            PreExecute();
            return @base.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            PreExecute();
            return @base.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            PreExecute();
            return @base.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            PreExecute();
            return @base.ExecuteScalar();
        }

        public void Prepare()
        {
            @base.Prepare();
        }
    }
}
