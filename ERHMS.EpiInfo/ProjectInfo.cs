using System.IO;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Location { get; set; }

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
