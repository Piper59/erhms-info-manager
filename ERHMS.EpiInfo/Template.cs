using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    XmlElement element = reader.ReadNextElement();
                    if (element != null && element.Name == "Template" && element.HasAllAttributes("Name", "Description", "Level"))
                    {
                        TemplateLevel level;
                        if (TemplateLevelExtensions.TryParse(element.GetAttribute("Level"), out level))
                        {
                            result = new Template(file, element.GetAttribute("Name"), element.GetAttribute("Description"), level);
                            return true;
                        }
                    }
                    return false;
                }
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
