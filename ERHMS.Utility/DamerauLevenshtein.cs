using System.Linq;

namespace ERHMS.Utility
{
    public static class DamerauLevenshtein
    {
        private static int Min(params int[] values)
        {
            return values.Min();
        }

        // https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        public static int GetDistance(string str1, string str2)
        {
            string a = (str1 ?? "").ToLower();
            string b = (str2 ?? "").ToLower();
            if (a == b)
            {
                return 0;
            }
            int m = a.Length;
            int n = b.Length;
            if (m == 0)
            {
                return n;
            }
            if (n == 0)
            {
                return m;
            }
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    int deletion = d[i - 1, j] + 1;
                    int insertion = d[i, j - 1] + 1;
                    int substitution = d[i - 1, j - 1] + cost;
                    d[i, j] = Min(deletion, insertion, substitution);
                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        int transposition = d[i - 2, j - 2] + cost;
                        d[i, j] = Min(d[i, j], transposition);
                    }
                }
            }
            return d[m, n];
        }
    }
}
