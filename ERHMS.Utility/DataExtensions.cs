using System.Data;
using System.Data.Common;

namespace ERHMS.Utility
{
    public static class DataExtensions
    {
        public static class OleDb
        {
            public static class FileExtensions
            {
                public const string Access = ".mdb";
            }

            public static class Providers
            {
                public const string Jet4 = "Microsoft.Jet.OLEDB.4.0";
            }
        }

        public static bool TableExists(this DbConnection @this, string name)
        {
            DataTable schema = @this.GetSchema("Tables", new string[] { null, null, name });
            return schema.Rows.Count > 0;
        }
    }
}
