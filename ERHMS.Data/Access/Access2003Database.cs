using ERHMS.Data.Properties;

namespace ERHMS.Data.Access
{
    public class Access2003Database : AccessDatabase
    {
        public static class Constants
        {
            public const string Provider = "Microsoft.Jet.OLEDB.4.0";
            public const string FileExtension = ".mdb";
        }

        protected override byte[] EmptyDatabase => Resources.Access2003Database;

        public Access2003Database(string connectionString)
            : base(DatabaseProvider.Access2003, connectionString) { }
    }
}
