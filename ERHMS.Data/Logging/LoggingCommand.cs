﻿using ERHMS.Common.Logging;
using log4net.Core;
using System.Data;
using System.Text.RegularExpressions;

namespace ERHMS.Data.Logging
{
    public class LoggingCommand : IDbCommand
    {
        private static readonly Regex lineBreakRegex = new Regex(@"(?:\r\n|\r|\n)\s*");

        public IDbCommand BaseCommand { get; }

        public IDbConnection Connection
        {
            get { return BaseCommand.Connection; }
            set { BaseCommand.Connection = value; }
        }

        public IDbTransaction Transaction
        {
            get { return BaseCommand.Transaction; }
            set { BaseCommand.Transaction = value; }
        }

        public string CommandText
        {
            get { return BaseCommand.CommandText; }
            set { BaseCommand.CommandText = value; }
        }

        private string CommandLogText => lineBreakRegex.Replace(CommandText, " ");

        public int CommandTimeout
        {
            get { return BaseCommand.CommandTimeout; }
            set { BaseCommand.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return BaseCommand.CommandType; }
            set { BaseCommand.CommandType = value; }
        }

        public IDataParameterCollection Parameters => BaseCommand.Parameters;

        public UpdateRowSource UpdatedRowSource
        {
            get { return BaseCommand.UpdatedRowSource; }
            set { BaseCommand.UpdatedRowSource = value; }
        }

        public Level Level { get; }

        public LoggingCommand(IDbCommand command, Level level)
        {
            BaseCommand = command;
            Level = level;
        }

        public void Cancel()
        {
            BaseCommand.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return BaseCommand.CreateParameter();
        }

        public void Dispose()
        {
            BaseCommand.Dispose();
        }

        private void OnExecuting()
        {
            Log.Instance.Log(Level, $"Executing database command: {CommandLogText}");
        }

        public int ExecuteNonQuery()
        {
            OnExecuting();
            return BaseCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            OnExecuting();
            return BaseCommand.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            OnExecuting();
            return BaseCommand.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            OnExecuting();
            return BaseCommand.ExecuteScalar();
        }

        public void Prepare()
        {
            BaseCommand.Prepare();
        }
    }
}
