using Epi;
using ERHMS.Utility;
using System;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        private static readonly Regex InvalidNameCharPattern = new Regex(@"[^a-zA-Z0-9_]");
        private static readonly Regex InvalidNameBeginningPattern = new Regex(@"^[^a-zA-Z]+");

        public static bool IsValidName(string viewName, out InvalidViewNameReason reason)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                reason = InvalidViewNameReason.Empty;
                return false;
            }
            else if (InvalidNameCharPattern.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidChar;
                return false;
            }
            else if (!char.IsLetter(viewName[0]))
            {
                reason = InvalidViewNameReason.InvalidFirstChar;
                return false;
            }
            else if (viewName.Length > 64)
            {
                reason = InvalidViewNameReason.TooLong;
                return false;
            }
            else
            {
                reason = InvalidViewNameReason.None;
                return true;
            }
        }

        public static string SanitizeName(string viewName)
        {
            return viewName.Strip(InvalidNameCharPattern).Strip(InvalidNameBeginningPattern);
        }

        public static bool IsWebSurvey(this View @this)
        {
            return !string.IsNullOrWhiteSpace(@this.WebSurveyId);
        }

        public static Uri GetWebSurveyUrl(this View @this)
        {
            Configuration configuration = Configuration.GetNewInstance();
            Uri endpoint = new Uri(configuration.Settings.WebServiceEndpointAddress);
            return new Uri(endpoint, string.Format("Home/{0}", @this.WebSurveyId));
        }
    }
}
