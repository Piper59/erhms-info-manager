using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
using System;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ExportLogs : IUtility
    {
        public string ArchivePath { get; }

        public ExportLogs(string archivePath)
        {
            ArchivePath = archivePath;
        }

        public void Run()
        {
            if (File.Exists(ArchivePath))
            {
                throw new InvalidOperationException("Archive already exists.");
            }
            ZipFileExtensions.CreateFromDirectory(FileAppender.Directory, ArchivePath, $"*{FileAppender.Extension}");
        }
    }
}
