using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Test
{
    public class TempDirectory : IDisposable
    {
        public string FullName { get; private set; }

        public TempDirectory(string directoryName)
        {
            FullName = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, directoryName);
            DeleteIfExists();
            Directory.CreateDirectory(FullName);
        }

        private void DeleteIfExists()
        {
            if (Directory.Exists(FullName))
            {
                Directory.Delete(FullName, true);
            }
        }

        public string CombinePaths(params string[] paths)
        {
            return Path.Combine(FullName, Path.Combine(paths));
        }

        public string CreateFile(params string[] paths)
        {
            string path = CombinePaths(paths);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (File.Create(path)) { }
            return path;
        }

        public void Dispose()
        {
            DeleteIfExists();
        }
    }
}
