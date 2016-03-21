using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";

        public static IEnumerable<Canvas> GetByProject(Project project)
        {
            string pattern = string.Format("*{0}", FileExtension);
            foreach (FileInfo file in project.Location.EnumerateFiles(pattern, SearchOption.AllDirectories))
            {
                yield return new Canvas(file);
            }
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
