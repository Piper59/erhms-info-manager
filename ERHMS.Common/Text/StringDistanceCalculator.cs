using System;

namespace ERHMS.Common.Text
{
    // https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance#Optimal_string_alignment_distance
    public static class StringDistanceCalculator
    {
        public static int GetDistance(string str1, string str2)
        {
            string a = str1.ToLower();
            string b = str2.ToLower();
            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= b.Length; j++)
            {
                d[0, j] = j;
            }
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    int deletion = d[i - 1, j] + 1;
                    int insertion = d[i, j - 1] + 1;
                    int substitution = d[i - 1, j - 1] + cost;
                    int d_ij = Math.Min(Math.Min(deletion, insertion), substitution);
                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        int transposition = d[i - 2, j - 2] + 1;
                        d_ij = Math.Min(d_ij, transposition);
                    }
                    d[i, j] = d_ij;
                }
            }
            return d[a.Length, b.Length];
        }

        public static double GetSimilarity(string str1, string str2)
        {
            int maxDistance = Math.Max(str1.Length, str2.Length);
            if (maxDistance == 0)
            {
                return 1.0;
            }
            else
            {
                int distance = GetDistance(str1, str2);
                return 1.0 - (double)distance / maxDistance;
            }
        }
    }
}
