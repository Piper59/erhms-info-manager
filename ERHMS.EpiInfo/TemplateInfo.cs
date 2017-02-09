using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class TemplateInfo
    {
        public const string FileExtension = ".xml";

        public static bool TryRead(FileInfo file, out TemplateInfo result)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    result = new TemplateInfo(file, reader.ReadNextElement());
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static TemplateInfo Get(FileInfo file)
        {
            TemplateInfo templateInfo;
            TryRead(file, out templateInfo);
            return templateInfo;
        }

        public static IEnumerable<TemplateInfo> GetAll(DirectoryInfo directory = null)
        {
            if (directory == null)
            {
                Configuration configuration = Configuration.GetNewInstance();
                directory = new DirectoryInfo(configuration.Directories.Templates);
            }
            foreach (FileInfo file in directory.SearchByExtension(FileExtension))
            {
                TemplateInfo templateInfo;
                if (TryRead(file, out templateInfo))
                {
                    yield return templateInfo;
                }
            }
        }

        public static IEnumerable<TemplateInfo> GetByLevel(TemplateLevel level, DirectoryInfo directory = null)
        {
            return GetAll(directory).Where(template => template.Level == level);
        }

        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TemplateLevel Level { get; private set; }

        private TemplateInfo(FileInfo file, XmlElement element)
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
