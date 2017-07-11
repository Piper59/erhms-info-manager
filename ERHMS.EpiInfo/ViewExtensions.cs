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
            else if (InvalidNameBeginningPattern.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidBeginning;
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

        public static string SanitizeName(string name)
        {
            return name.Strip(InvalidNameCharPattern).Strip(InvalidNameBeginningPattern);
        }

        public static bool IsWebSurvey(this View @this)
        {
            return !string.IsNullOrWhiteSpace(@this.WebSurveyId);
        }

        public static Uri GetWebSurveyUrl(this View @this)
        {
            Uri endpoint = new Uri(Configuration.GetNewInstance().Settings.WebServiceEndpointAddress);
            return new Uri(endpoint, string.Format("Home/{0}", @this.WebSurveyId));
        }
    }
}
