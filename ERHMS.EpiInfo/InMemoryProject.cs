using System.Data.Common;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class InMemoryProject : Project
    {
        public InMemoryProject(string name, DirectoryInfo location, string driver, DbConnectionStringBuilder builder)
            : base(name, location, driver, builder)
        { }

        public override void Save() { }
    }
}
