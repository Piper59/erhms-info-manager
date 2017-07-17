using ERHMS.Dapper;

namespace ERHMS.Test.Dapper
{
    public class DataContext
    {
        public IDatabase Database { get; private set; }
        public Repository<Constant, int> Constants { get; private set; }
        public Repository<Gender, string> Genders { get; private set; }
        public PersonRepository People { get; private set; }

        public DataContext(IDatabase database)
        {
            Database = database;
            Constants = new Repository<Constant, int>(database);
            Genders = new Repository<Gender, string>(database);
            People = new PersonRepository(database);
        }
    }
}
