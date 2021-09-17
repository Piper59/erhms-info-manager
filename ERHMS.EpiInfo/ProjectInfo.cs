using System.IO;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public string Name { get; set; }

        private string location;
        public string Location
        {
            get
            {
                if (location != null)
                {
                    return location;
                }
                if (locationRoot != null)
                {
                    return Path.Combine(locationRoot, Name);
                }
                return null;
            }
            set
            {
                location = value;
            }
        }

        private string locationRoot;
        public string LocationRoot
        {
            get
            {
                if (locationRoot != null)
                {
                    return locationRoot;
                }
                if (location != null)
                {
                    return Path.GetDirectoryName(location);
                }
                return null;
            }
            set
            {
                locationRoot = value;
            }
        }

        private string filePath;
        public string FilePath
        {
            get
            {
                if (filePath != null)
                {
                    return filePath;
                }
                if (Location != null)
                {
                    return Path.Combine(Location, $"{Name}{FileExtensions.Project}");
                }
                return null;
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
