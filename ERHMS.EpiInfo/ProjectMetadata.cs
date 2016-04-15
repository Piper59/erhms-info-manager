using Epi;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class ProjectMetadata
    {
        public static bool TryRead(FileInfo file, out ProjectMetadata metadata)
        {
            try
            {
                string name = null;
                string description = null;
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name != "Project")
                            {
                                metadata = null;
                                return false;
                            }
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "name":
                                        name = reader.Value;
                                        break;
                                    case "description":
                                        description = reader.Value;
                                        break;
                                }
                            }
                            break;
                        }
                    }
                }
                metadata = new ProjectMetadata(file, name, description);
                return true;
            }
            catch
            {
                metadata = null;
                return false;
            }
        }

        public static IEnumerable<ProjectMetadata> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Project);
            string pattern = string.Format("*{0}", Project.FileExtension);
            foreach (FileInfo file in directory.EnumerateFiles(pattern, SearchOption.AllDirectories))
            {
                ProjectMetadata metadata;
                if (TryRead(file, out metadata))
                {
                    yield return metadata;
                }
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
