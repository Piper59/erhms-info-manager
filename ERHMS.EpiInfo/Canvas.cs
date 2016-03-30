using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";

        public static bool TryRead(FileInfo file, out Canvas canvas)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(file.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name != "DashboardCanvas")
                            {
                                canvas = null;
                                return false;
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                canvas = null;
                return false;
            }
            canvas = new Canvas(file);
            return true;
        }

        public static IEnumerable<Canvas> GetByProject(Project project)
        {
            string pattern = string.Format("*{0}", FileExtension);
            foreach (FileInfo file in project.Location.EnumerateFiles(pattern, SearchOption.AllDirectories))
            {
                Canvas canvas;
                if (TryRead(file, out canvas))
                {
                    yield return canvas;
                }
            }
        }

        public static Canvas CreateForView(View view, FileInfo file)
        {
            Templates.Canvas template = new Templates.Canvas(view.Project.FilePath, view.Name);
            System.IO.File.WriteAllText(file.FullName, template.TransformText());
            return new Canvas(file);
        }

        public FileInfo File { get; private set; }

        private Canvas(FileInfo file)
        {
            File = file;
        }

        public void Delete()
        {
            File.Recycle();
        }
    }
}
