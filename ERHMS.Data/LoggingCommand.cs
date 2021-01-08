using ERHMS.Common.Logging;
using System.Data;
using System.Text.RegularExpressions;

namespace ERHMS.Data
{
    public class LoggingCommand : IDbCommand
    {
        private static readonly Regex newLineAndIndentationRegex = new Regex(@"(?:\r\n?|\n)\s*");

        public IDbCommand UnderlyingCommand { get; }

        public IDbConnection Connection
        {
            get { return UnderlyingCommand.Connection; }
            set { UnderlyingCommand.Connection = value; }
        }

        public IDbTransaction Transaction
        {
            get { return UnderlyingCommand.Transaction; }
            set { UnderlyingCommand.Transaction = value; }
        }

        public string CommandText
        {
            get { return UnderlyingCommand.CommandText; }
            set { UnderlyingCommand.CommandText = value; }
        }

        public string CommandLogText => newLineAndIndentationRegex.Replace(CommandText, "");

        public int CommandTimeout
        {
            get { return UnderlyingCommand.CommandTimeout; }
            set { UnderlyingCommand.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return UnderlyingCommand.CommandType; }
            set { UnderlyingCommand.CommandType = value; }
        }

        public IDataParameterCollection Parameters => UnderlyingCommand.Parameters;

        public UpdateRowSource UpdatedRowSource
        {
            get { return UnderlyingCommand.UpdatedRowSource; }
            set { UnderlyingCommand.UpdatedRowSource = value; }
        }

        public LoggingCommand(IDbCommand command)
        {
            UnderlyingCommand = command;
        }

        public void Prepare()
        {
            UnderlyingCommand.Prepare();
        }

        public void Cancel()
        {
            UnderlyingCommand.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return UnderlyingCommand.CreateParameter();
        }

        private void OnExecuting()
        {
            Log.Instance.Debug($"Executing database command: {CommandLogText}");
        }

        public int ExecuteNonQuery()
        {
            OnExecuting();
            return UnderlyingCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            OnExecuting();
            return UnderlyingCommand.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            OnExecuting();
            return UnderlyingCommand.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            OnExecuting();
            return UnderlyingCommand.ExecuteScalar();
        }

        public void Dispose()
        {
            UnderlyingCommand.Dispose();
        }
    }
}
