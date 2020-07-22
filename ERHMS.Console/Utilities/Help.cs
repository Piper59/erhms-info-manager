using System.IO;
using System.Linq;

namespace ERHMS.Console.Utilities
{
    public class Help : Utility
    {
        private TextWriter writer;
        public TextWriter Writer
        {
            get { return writer ?? Out; }
            set { writer = value; }
        }

        protected override void RunCore()
        {
            Writer.WriteLine("The following utilities are available:");
            foreach (string typeName in Types.Keys.OrderBy(typeName => typeName))
            {
                Writer.WriteLine($"  {typeName}");
            }
        }
    }
}
