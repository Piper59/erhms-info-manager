using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        private static readonly Regex InvalidViewNameCharacter = new Regex("[^a-zA-Z0-9]");

        public static string SanitizeName(string viewName)
        {
            return InvalidViewNameCharacter.Replace(viewName, "");
        }
    }
}
