using ERHMS.Utility;
using System.Data.Common;

namespace ERHMS.EpiInfo
{
    public class ProjectCreationInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Driver { get; set; }
        public DbConnectionStringBuilder Builder { get; set; }
        public string DatabaseName { get; set; }
        public bool Initialize { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Name, Location, Driver, Builder.GetCensoredConnectionString(), DatabaseName);
        }
    }
}
