using Epi;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class ProjectMetadata
    {
        public static IEnumerable<ProjectMetadata> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Project);
            string pattern = string.Format("*{0}", Project.FileExtension);
            foreach (FileInfo file in directory.EnumerateFiles(pattern, SearchOption.AllDirectories))
            {
                string name;
                string description;
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    reader.ReadToFollowing("Project");
                    reader.MoveToAttribute("name");
                    name = reader.Value;
                    reader.MoveToAttribute("description");
                    description = reader.Value;
                }
                yield return new ProjectMetadata(file, name, description);
            }
        }

        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private ProjectMetadata(FileInfo file, string name, string description)
        {
            File = file;
            Name = name;
            Description = description;
        }

        public Project OpenProject()
        {
            return new Project(File);
        }
    }
}
