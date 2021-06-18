using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
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
            ZipFileExtensions.CreateFromDirectory(
                FileAppender.Directory,
                ArchivePath,
                $"*{FileAppender.Extension}",
                FileMode.Create,
                FileShare.ReadWrite);
        }
    }
}
