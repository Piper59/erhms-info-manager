using ERHMS.Data.Properties;

namespace ERHMS.Data.Access
{
    public class Access2007Database : AccessDatabase
    {
        protected override byte[] EmptyDatabase => Resources.Access2007Database;

        public Access2007Database(string connectionString)
            : base(DatabaseProvider.Access2007, connectionString) { }
    }
}
