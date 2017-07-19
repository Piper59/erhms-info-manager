using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
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
            ProjectInfo result;
            TryRead(path, out result);
            return result;
        }

        public static IEnumerable<ProjectInfo> GetAll()
        {
            DirectoryInfo directory = new DirectoryInfo(Configuration.GetNewInstance().Directories.Project);
            foreach (FileInfo file in directory.SearchByExtension(Project.FileExtension))
            {
                ProjectInfo result;
                if (TryRead(file.FullName, out result))
                {
                    yield return result;
                }
            }
        }

        public string FilePath { get; private set; }
        public Version Version { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private ProjectInfo(string path, XmlElement element)
        {
            if (element.Name != "Project" || !element.HasAllAttributes("name", "description"))
            {
                throw new ArgumentException("Element is not a valid project.", nameof(element));
            }
            FilePath = path;
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
            XmlDocument document = new XmlDocument();
            document.Load(FilePath);
            XmlElement element = document.SelectSingleElement("/Project/CollectedData/Database");
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.Providers.Jet4,
                DataSource = Path.ChangeExtension(FilePath, ".mdb")
            };
            element.SetAttribute("connectionString", Configuration.Encrypt(builder.ConnectionString));
            document.Save(FilePath);
        }
    }
}
