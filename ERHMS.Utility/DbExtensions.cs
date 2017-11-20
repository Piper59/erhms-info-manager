namespace ERHMS.Utility
{
    public static class DbExtensions
    {
        public static string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public static string GetParameterName(int index)
        {
            return "@P" + index;
        }
    }
}
