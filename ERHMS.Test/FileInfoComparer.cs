using System.Collections;
using System.IO;

namespace ERHMS.Test
{
    public class FileInfoComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null || y == null)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                else
                {
                    return x == null ? -1 : 1;
                }
            }
            else
            {
                return string.Compare(((FileInfo)x).FullName, ((FileInfo)y).FullName);
            }
        }
    }
}
