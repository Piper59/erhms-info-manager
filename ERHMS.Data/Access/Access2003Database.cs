using ERHMS.Data.Properties;
using System.IO;

namespace ERHMS.Data.Access
{
    public class Access2003Database : AccessDatabase
    {
        public static class Constants
        {
            public const string Provider = "Microsoft.Jet.OLEDB.4.0";
            public const string FileExtension = ".mdb";
        }

        public Access2003Database(string connectionString)
            : base(DatabaseProvider.Access2003, connectionString) { }

        protected override void CreateCore(Stream stream)
        {
            stream.Write(ResX.Access2003Database, 0, ResX.Access2003Database.Length);
        }
    }
}
