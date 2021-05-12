using ERHMS.Data.Properties;

namespace ERHMS.Data.Access
{
    public class Access2003Database : AccessDatabase
    {
        protected override byte[] EmptyDatabase => Resources.Access2003Database;

        public Access2003Database(string connectionString)
            : base(DatabaseProvider.Access2003, connectionString) { }
    }
}
