using ERHMS.Common.Logging;
using System;
using System.IO;
using System.Xml.Linq;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        private static string GetDescription(string path)
        {
            try
            {
                XDocument document = XDocument.Load(path);
                return document.Root.Attribute("description").Value;
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
                return null;
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }

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
                Description = GetDescription(value);
            }
        }

        public ProjectInfo() { }

        public ProjectInfo(string filePath)
        {
            FilePath = filePath;
        }
    }
}
