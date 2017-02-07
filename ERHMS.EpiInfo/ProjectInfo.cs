using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class ProjectInfo
    {
        public static bool TryRead(FileInfo file, out ProjectInfo result)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    result = new ProjectInfo(file, reader.ReadNextElement());
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static ProjectInfo Get(FileInfo file)
        {
            ProjectInfo projectInfo;
            TryRead(file, out projectInfo);
            return projectInfo;
        }

        public static IEnumerable<ProjectInfo> GetAll(DirectoryInfo directory = null)
        {
            if (directory == null)
            {
                Configuration configuration = Configuration.GetNewInstance();
                directory = new DirectoryInfo(configuration.Directories.Project);
            }
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
        public Version Version { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private ProjectInfo(FileInfo file, XmlElement element)
        {
            if (element.Name != "Project" || !element.HasAllAttributes("name", "description"))
            {
                throw new ArgumentException("Element is not a valid project.");
            }
            File = file;
            Version version;
            if (Version.TryParse(element.GetAttribute("erhmsVersion"), out version))
            {
                Version = version;
            }
            Name = element.GetAttribute("name");
            Description = element.GetAttribute("description");
        }
    }
}
