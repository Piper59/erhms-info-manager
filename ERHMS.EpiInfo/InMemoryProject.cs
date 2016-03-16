using System.Data.Common;

namespace ERHMS.EpiInfo
{
    public class InMemoryProject : ProjectBase
    {
        public InMemoryProject(string driver, DbConnectionStringBuilder builder)
            : base(driver, builder)
        { }

        public override void Save() { }
    }
}
