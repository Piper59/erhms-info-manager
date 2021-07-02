using System.IO;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string FilePath => Path.Combine(Location, $"{Name}{FileExtensions.Project}");
    }
}
