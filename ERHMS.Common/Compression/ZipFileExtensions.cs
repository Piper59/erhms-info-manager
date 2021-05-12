using System;
using System.IO;
using System.IO.Compression;

namespace ERHMS.Common.Compression
{
    public static class ZipFileExtensions
    {
        public static void CreateFromDirectory(
            string directoryPath,
            string archivePath,
            string searchPattern = "*",
            FileShare fileShare = FileShare.Read)
        {
            using (Stream archiveStream = File.Open(archivePath, FileMode.Create, FileAccess.Write))
            using (ZipArchive archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                Uri directoryUri = new Uri(directory.FullName);
                foreach (FileInfo file in directory.EnumerateFiles(searchPattern, SearchOption.AllDirectories))
                {
                    Uri fileUri = new Uri(file.FullName);
                    string entryName = directoryUri.MakeRelativeUri(fileUri).ToString();
                    ZipArchiveEntry entry = archive.CreateEntry(entryName);
                    entry.LastWriteTime = file.LastWriteTime;
                    using (Stream fileStream = file.Open(FileMode.Open, FileAccess.Read, fileShare))
                    using (Stream entryStream = entry.Open())
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }
        }
    }
}
