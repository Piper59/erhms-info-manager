using System;

namespace ERHMS.Data
{
    public abstract class Repository : IDisposable
    {
        private readonly IConnector connector;

        protected IDatabase Database { get; }

        protected Repository(IDatabase database)
        {
            Database = database;
            connector = database.Connect();
        }

        protected string Quote(string identifier)
        {
            return Database.Quote(identifier);
        }

        public ITransactor Transact()
        {
            return Database.Transact();
        }

        public void Dispose()
        {
            connector.Dispose();
        }
    }
}
