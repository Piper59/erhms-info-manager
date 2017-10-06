using System;
using System.Data;

namespace ERHMS.Dapper
{
    public class ConnectionOpener : IDisposable
    {
        private bool closed;

        public IDbConnection Connection { get; private set; }

        public ConnectionOpener(IDbConnection connection)
        {
            Connection = connection;
            closed = connection.State == ConnectionState.Closed;
            if (closed)
            {
                connection.Open();
            }
        }

        public void Dispose()
        {
            if (closed && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }
    }
}
