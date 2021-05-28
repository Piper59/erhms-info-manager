using Epi;
using System.IO;

namespace ERHMS.EpiInfo.Analytics
{
    public class Pgm : Asset
    {
        public Pgm(View view)
            : base(view) { }

        public override void Save(Stream stream)
        {
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"READ {{{View.Project.FilePath}}}:{View.Name}");
            }
        }
    }
}
