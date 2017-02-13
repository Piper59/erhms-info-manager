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

        public static bool TryRead(string path, out TemplateInfo result)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    result = new TemplateInfo(path, reader.ReadNextElement());
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static TemplateInfo Get(string path)
        {
            TemplateInfo templateInfo;
            TryRead(path, out templateInfo);
            return templateInfo;
        }

        public static IEnumerable<TemplateInfo> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Templates);
            foreach (FileInfo file in directory.Search(FileExtension))
            {
                TemplateInfo templateInfo;
                if (TryRead(file.FullName, out templateInfo))
                {
                    yield return templateInfo;
                }
            }
        }

        public static IEnumerable<TemplateInfo> GetByLevel(TemplateLevel level)
        {
            return GetAll().Where(template => template.Level == level);
        }

        public string Path { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TemplateLevel Level { get; private set; }

        private TemplateInfo(string path, XmlElement element)
        {
            if (element.Name != "Template" || !element.HasAllAttributes("Name", "Description", "Level"))
            {
                throw new ArgumentException("Element is not a valid template.");
            }
            Path = path;
            Name = element.GetAttribute("Name");
            Description = element.GetAttribute("Description");
            Level = TemplateLevelExtensions.Parse(element.GetAttribute("Level"));
        }

        public void Delete()
        {
            IOExtensions.RecycleFile(Path);
        }
    }
}
