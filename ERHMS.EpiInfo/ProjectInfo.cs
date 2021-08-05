using System.IO;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public string Name { get; set; }

        private string location;
        public string Location
        {
            get { return location ?? Path.Combine(locationRoot, Name); }
            set { location = value; }
        }

        private string locationRoot;
        public string LocationRoot
        {
            get { return locationRoot ?? Path.GetDirectoryName(location); }
            set { locationRoot = value; }
        }

        private string filePath;
        public string FilePath
        {
            get
            {
                return filePath ?? Path.Combine(Location, $"{Name}{FileExtensions.Project}");
            }
            set
            {
                filePath = value;
                Name = Path.GetFileNameWithoutExtension(value);
                Location = Path.GetDirectoryName(value);
            }
        }

        public ProjectInfo() { }

        public ProjectInfo(string filePath)
        {
            FilePath = filePath;
        }
    }
}
