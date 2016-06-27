using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class Template
    {
        public const string FileExtension = ".xml";

        public static bool TryRead(FileInfo file, out Template result)
        {
            result = null;
            try
            {
                string name = null;
                string description = null;
                string levelString = null;
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name != "Template")
                            {
                                return false;
                            }
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "Name":
                                        name = reader.Value;
                                        break;
                                    case "Description":
                                        description = reader.Value;
                                        break;
                                    case "Level":
                                        levelString = reader.Value;
                                        break;
                                }
                            }
                            break;
                        }
                    }
                }
                if (name == null || description == null || levelString == null)
                {
                    return false;
                }
                TemplateLevel level;
                if (!TemplateLevelExtensions.TryParse(levelString, out level))
                {
                    return false;
                }
                result = new Template(file, name, description, level);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Template Get(FileInfo file)
        {
            Template template;
            TryRead(file, out template);
            return template;
        }

        public static Template Get(TemplateLevel level, string name)
        {
            Configuration configuration = Configuration.GetNewInstance();
            FileInfo file = new FileInfo(Path.Combine(
                configuration.Directories.Templates,
                level.ToDirectoryName(),
                string.Format("{0}{1}", name, FileExtension)));
            return Get(file);
        }

        public static IEnumerable<Template> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Templates);
            foreach (FileInfo file in directory.SearchByExtension(FileExtension))
            {
                Template template;
                if (TryRead(file, out template))
                {
                    yield return template;
                }
            }
        }

        public static IEnumerable<Template> GetByLevel(TemplateLevel level)
        {
            return GetAll().Where(template => template.Level == level);
        }

        public static Template GetFromResource(Assembly assembly, string resourceName)
        {
            FileInfo file = IOExtensions.GetTemporaryFile(extension: ".xml");
            System.IO.File.WriteAllText(file.FullName, assembly.GetManifestResourceText(resourceName));
            return Get(file);
        }

        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TemplateLevel Level { get; private set; }

        private Template(FileInfo file, string name, string description, TemplateLevel level)
        {
            File = file;
            Name = name;
            Description = description;
            Level = level;
        }

        public void Delete()
        {
            File.Recycle();
        }
    }
}
