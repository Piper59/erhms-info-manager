using Epi;
using System.IO;

namespace ERHMS.EpiInfo.Analysis
{
    public class Program
    {
        public View View { get; }

        public Program(View view)
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
