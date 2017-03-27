﻿using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Test
{
    using IOPath = Path;

    public class TempDirectory : IDisposable
    {
        public string Path { get; private set; }

        public TempDirectory(string directoryName)
        {
            Path = IOPath.Combine(IOPath.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, directoryName);
            DeleteIfExists();
            Directory.CreateDirectory(Path);
        }

        private void DeleteIfExists()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }
        }

        public string CombinePaths(params string[] paths)
        {
            return IOPath.Combine(Path, IOPath.Combine(paths));
        }

        public string CreateFile(params string[] paths)
        {
            string path = CombinePaths(paths);
            Directory.CreateDirectory(IOPath.GetDirectoryName(path));
            using (File.Create(path)) { }
            return path;
        }

        public void Dispose()
        {
            DeleteIfExists();
        }
    }
}
