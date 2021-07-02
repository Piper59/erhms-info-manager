using Microsoft.VisualBasic.FileIO;

namespace ERHMS.Common.IO
{
    public static class FileSystemExtensions
    {
        public static void Recycle(string path)
        {
            FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
    }
}
