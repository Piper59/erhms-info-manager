using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ExportLogs : IUtility
    {
        public string OutputPath { get; }

        public ExportLogs(string outputPath)
        {
            OutputPath = outputPath;
        }

        public void Run()
        {
            ZipFileExtensions.CreateFromDirectory(
                FileAppender.Directory,
                OutputPath,
                $"*{FileAppender.Extension}",
                FileMode.Create,
                FileShare.ReadWrite);
        }
    }
}
