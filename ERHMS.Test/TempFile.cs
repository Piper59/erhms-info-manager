using System;
using System.IO;

namespace ERHMS.Test
{
    public class TempFile : IDisposable
    {
        public string FullName { get; private set; }

        public TempFile()
        {
            FullName = Path.GetTempFileName();
        }

        public TempFile(string path)
        {
            using (File.Create(path)) { }
            FullName = path;
        }

        public void Dispose()
        {
            File.Delete(FullName);
        }
    }
}
