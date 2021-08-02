using System.IO;

namespace ERHMS.Common.IO
{
    public static class PathExtensions
    {
        public static string TrimEnd(string path)
        {
            if (Path.GetDirectoryName(path) == null)
            {
                return path;
            }
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
