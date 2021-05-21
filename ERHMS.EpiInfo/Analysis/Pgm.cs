using Epi;
using System.IO;

namespace ERHMS.EpiInfo.Analysis
{
    public class Pgm
    {
        public View View { get; }

        public Pgm(View view)
        {
            View = view;
        }

        public void Save(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create, FileAccess.Write))
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"READ {{{View.Project.FilePath}}}:{View.Name}");
            }
        }
    }
}
