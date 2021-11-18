using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ExportLogs : Utility
    {
        public string OutputPath { get; }

        public ExportLogs(string outputPath)
        {
            OutputPath = outputPath;
        }

        public override void Run()
        {
            ZipFileExtensions.CreateFromDirectory(
                FileAppender.Directory,
                OutputPath,
                true,
                $"*{FileAppender.Extension}",
                FileShare.ReadWrite);
        }
    }
}
