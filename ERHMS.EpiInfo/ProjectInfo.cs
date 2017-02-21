using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    using IOPath = Path;

    public class ProjectInfo
    {
        public static bool TryRead(string path, out ProjectInfo result)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    result = new ProjectInfo(path, reader.ReadNextElement());
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static ProjectInfo Get(string path)
        {
            ProjectInfo projectInfo;
            TryRead(path, out projectInfo);
            return projectInfo;
        }

        public static IEnumerable<ProjectInfo> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Project);
            foreach (FileInfo file in directory.Search(Project.FileExtension))
            {
                ProjectInfo projectInfo;
                if (TryRead(file.FullName, out projectInfo))
                {
                    yield return projectInfo;
                }
            }
        }

        public string Path { get; private set; }
        public Version Version { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private ProjectInfo(string path, XmlElement element)
        {
            if (element.Name != "Project" || !element.HasAllAttributes("name", "description"))
            {
                throw new ArgumentException("Element is not a valid project.");
            }
            Path = path;
            Version version;
            if (Version.TryParse(element.GetAttribute("erhmsVersion"), out version))
            {
                Version = version;
            }
            Name = element.GetAttribute("name");
            Description = element.GetAttribute("description");
        }

        public void SetAccessConnectionString()
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.Jet.OLEDB.4.0",
                DataSource = IOPath.ChangeExtension(Path, ".mdb")
            };
            XmlDocument document = new XmlDocument();
            document.Load(Path);
            XmlElement databaseElement = document.SelectSingleElement("/Project/CollectedData/Database");
            databaseElement.SetAttribute("connectionString", Configuration.Encrypt(builder.ConnectionString));
            document.Save(Path);
        }
    }
}
