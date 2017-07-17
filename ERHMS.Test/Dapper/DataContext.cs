using ERHMS.Dapper;

namespace ERHMS.Test.Dapper
{
    public class DataContext
    {
        public IDatabase Database { get; private set; }
        public IRepository<Constant> Constants { get; private set; }
        public IRepository<Gender> Genders { get; private set; }
        public IRepository<Person> People { get; private set; }

        public DataContext(IDatabase database)
        {
            Database = database;
            Constants = new Repository<Constant>(database);
            Genders = new Repository<Gender>(database);
            People = new PersonRepository(database);
        }
    }
}
