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

        public static bool TryRead(FileInfo file, out Template template)
        {
            try
            {
                string levelString = null;
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "Template" && reader.MoveToAttribute("Level"))
                            {
                                levelString = reader.Value;
                            }
                            break;
                        }
                    }
                }
                TemplateLevel level;
                if (!TemplateLevelExtensions.TryParse(levelString, out level))
                {
                    template = null;
                    return false;
                }
                template = new Template(file, level);
                return true;
            }
            catch
            {
                template = null;
                return false;
            }
        }

        public static IEnumerable<Template> GetAll()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.Templates);
            string pattern = string.Format("*{0}", FileExtension);
            foreach (FileInfo file in directory.EnumerateFiles(pattern, SearchOption.AllDirectories))
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
        public TemplateLevel Level { get; private set; }

        private Template(FileInfo file, TemplateLevel level)
        {
            File = file;
            Level = level;
        }

        public void Delete()
        {
            File.Recycle();
        }
    }
}
