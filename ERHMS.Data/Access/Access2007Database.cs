using ERHMS.Data.Properties;

namespace ERHMS.Data.Access
{
    public class Access2007Database : AccessDatabase
    {
        public static class Constants
        {
            public const string Provider = "Microsoft.ACE.OLEDB.12.0";
            public const string FileExtension = ".accdb";
        }

        protected override byte[] EmptyDatabase => Resources.Access2007Database;

        public Access2007Database(string connectionString)
            : base(DatabaseProvider.Access2007, connectionString) { }
    }
}
