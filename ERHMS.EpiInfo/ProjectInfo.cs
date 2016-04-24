using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public static bool TryRead(FileInfo file, out ProjectInfo result)
        {
            result = null;
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
                if (name == null || description == null)
                {
                    return false;
                }
                result = new ProjectInfo(file, name, description);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<ProjectInfo> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Project);
            foreach (FileInfo file in directory.SearchByExtension(Project.FileExtension))
            {
                ProjectInfo projectInfo;
                if (TryRead(file, out projectInfo))
                {
                    yield return projectInfo;
                }
            }
        }

        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private ProjectInfo(FileInfo file, string name, string description)
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
