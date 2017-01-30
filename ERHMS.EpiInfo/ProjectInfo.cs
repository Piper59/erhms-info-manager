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
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    XmlElement element = reader.ReadNextElement();
                    if (element != null && element.Name == "Project" && element.HasAllAttributes("name", "description"))
                    {
                        result = new ProjectInfo(file, element.GetAttribute("name"), element.GetAttribute("description"));
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static ProjectInfo Get(FileInfo file)
        {
            ProjectInfo projectInfo;
            TryRead(file, out projectInfo);
            return projectInfo;
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
    }
}
