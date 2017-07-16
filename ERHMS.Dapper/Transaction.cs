using System;
using System.Data;

namespace ERHMS.Dapper
{
    public class Transaction : IDbTransaction
    {
        private bool open;

        public IDbTransaction Base { get; private set; }

        public IDbConnection Connection
        {
            get { return Base.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return Base.IsolationLevel; }
        }

        public Transaction(IDbTransaction @base)
        {
            Base = @base;
            open = true;
        }

        public event EventHandler Closed;
        private void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }
        private void OnClosed()
        {
            OnClosed(EventArgs.Empty);
        }

        private void Close()
        {
            open = false;
            OnClosed();
        }

        public void Commit()
        {
            Base.Commit();
            Close();
        }

        public void Dispose()
        {
            if (open)
            {
                Rollback();
            }
            Base.Dispose();
        }

        public void Rollback()
        {
            Base.Rollback();
            Close();
        }
    }
}
