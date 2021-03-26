using System.Linq;

namespace ERHMS.Common
{
    // https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance#Optimal_string_alignment_distance
    public static class StringDistanceCalculator
    {
        private class OneIndexedString
        {
            public string Value { get; }
            public int Length => Value.Length;
            public char this[int index] => Value[index - 1];

            public OneIndexedString(string value)
            {
                Value = value;
            }
        }

        private static int Min(params int[] values)
        {
            return values.Min();
        }

        public static int GetDistance(string str1, string str2)
        {
            OneIndexedString a = new OneIndexedString(str1.ToLower());
            OneIndexedString b = new OneIndexedString(str2.ToLower());
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
                    int cost = a[i] == b[j] ? 0 : 1;
                    int deletion = d[i - 1, j] + 1;
                    int insertion = d[i, j - 1] + 1;
                    int substitution = d[i - 1, j - 1] + cost;
                    d[i, j] = Min(deletion, insertion, substitution);
                    if (i > 1 && j > 1 && a[i] == b[j - 1] && a[i - 1] == b[j])
                    {
                        int transposition = d[i - 2, j - 2] + 1;
                        d[i, j] = Min(d[i, j], transposition);
                    }
                }
            }
            return d[a.Length, b.Length];
        }
    }
}
