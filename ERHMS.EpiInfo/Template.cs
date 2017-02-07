using Epi;
using ERHMS.Utility;
using System;
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
            try
            {
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    result = new Template(file, reader.ReadNextElement());
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static Template Get(FileInfo file)
        {
            Template template;
            TryRead(file, out template);
            return template;
        }

        public static IEnumerable<Template> GetAll(DirectoryInfo directory = null)
        {
            if (directory == null)
            {
                Configuration configuration = Configuration.GetNewInstance();
                directory = new DirectoryInfo(configuration.Directories.Templates);
            }
            foreach (FileInfo file in directory.SearchByExtension(FileExtension))
            {
                Template template;
                if (TryRead(file, out template))
                {
                    yield return template;
                }
            }
        }

        public static IEnumerable<Template> GetByLevel(TemplateLevel level, DirectoryInfo directory = null)
        {
            return GetAll(directory).Where(template => template.Level == level);
        }

        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TemplateLevel Level { get; private set; }

        private Template(FileInfo file, XmlElement element)
        {
            if (element.Name != "Template" || !element.HasAllAttributes("Name", "Description", "Level"))
            {
                throw new ArgumentException("Element is not a valid template.");
            }
            File = file;
            Name = element.GetAttribute("Name");
            Description = element.GetAttribute("Description");
            Level = TemplateLevelExtensions.Parse(element.GetAttribute("Level"));
        }

        public void Delete()
        {
            File.Recycle();
        }
    }
}
