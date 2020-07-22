using Epi;
using ERHMS.Data;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class ProjectCreationInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private string location;
        public string Location
        {
            get { return location; }
            set { location = Path.GetFullPath(value); }
        }

        public string FilePath => Path.Combine(Location, $"{Name}{FileExtensions.EPI_PROJ}");
        public IDatabase Database { get; set; }
    }
}
