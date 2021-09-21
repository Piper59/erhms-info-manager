using ERHMS.Common.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.Common.IO
{
    public static class PathExtensions
    {
        private static readonly char[] directorySeparatorChars = new char[]
        {
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        };

        public static bool IsPathRoot(string path)
        {
            return path != null && Path.GetDirectoryName(path) == null;
        }

        public static string TrimEnd(string path)
        {
            return IsPathRoot(path) ? path : path.TrimEnd(directorySeparatorChars);
        }

        public static IEnumerable<string> Compact(string path)
        {
            yield return path;
            IList<string> segments = path.Split(directorySeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Count <= 2)
            {
                yield break;
            }
            for (int startIndex = 2; startIndex < segments.Count; startIndex++)
            {
                yield return string.Join(
                    Path.DirectorySeparatorChar.ToString(),
                    segments.Where((segment, index) => index >= startIndex).Prepend(segments[0], "..."));
            }
        }
    }
}
