using System;
using System.Data.Common;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataTransaction : IDisposable
    {
        private DbTransaction @base;

        public DataTransactionState State { get; private set; }

        public DataTransaction(DbConnection connection)
        {
            @base = connection.BeginTransaction();
            State = DataTransactionState.Open;
        }

        public event EventHandler Completed;
        private void OnCompleted(EventArgs e)
        {
            EventHandler handler = Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnCompleted()
        {
            OnCompleted(EventArgs.Empty);
        }

        public DbCommand CreateCommand()
        {
            DbCommand command = @base.Connection.CreateCommand();
            command.Transaction = @base;
            return command;
        }

        public void Commit()
        {
            @base.Commit();
            State = DataTransactionState.Committed;
            OnCompleted();
        }

        public void Rollback()
        {
            @base.Rollback();
            State = DataTransactionState.RolledBack;
            OnCompleted();
        }

        public void Dispose()
        {
            if (State == DataTransactionState.Open)
            {
                Rollback();
            }
            @base.Dispose();
        }
    }
}
