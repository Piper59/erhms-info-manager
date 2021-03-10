using ERHMS.Data.Properties;
using System.IO;

namespace ERHMS.Data.Access
{
    public class Access2007Database : AccessDatabase
    {
        public static class Constants
        {
            public const string Provider = "Microsoft.ACE.OLEDB.12.0";
            public const string FileExtension = ".accdb";
        }

        public Access2007Database(string connectionString)
            : base(DatabaseProvider.Access2007, connectionString) { }

        protected override void CreateCore(Stream stream)
        {
            stream.Write(Resources.Access2007Database, 0, Resources.Access2007Database.Length);
        }
    }
}
