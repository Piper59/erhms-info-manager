using Epi;
using System;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        private static readonly Regex InvalidNameChar = new Regex(@"[^a-zA-Z0-9_]");

        public static bool IsValidName(string viewName, out InvalidViewNameReason reason)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                reason = InvalidViewNameReason.Empty;
                return false;
            }
            else if (InvalidNameChar.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidChar;
                return false;
            }
            else if (!char.IsLetter(viewName[0]))
            {
                reason = InvalidViewNameReason.InvalidFirstChar;
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
            if (char.IsLetter(viewName[0]))
            {
                return InvalidNameChar.Replace(viewName, "");
            }
            else
            {
                throw new ArgumentException("View name does not begin with a letter.");
            }
        }

        public static void EnsureDataTablesExist(this View @this)
        {
            if (!@this.Project.CollectedData.TableExists(@this.TableName))
            {
                @this.Project.CollectedData.CreateDataTableForView(@this, 1);
            }
        }

        public static bool IsWebSurvey(this View @this)
        {
            return !string.IsNullOrEmpty(@this.WebSurveyId);
        }

        public static Uri GetWebSurveyUrl(this View @this)
        {
            Configuration configuration = Configuration.GetNewInstance();
            Uri endpoint = new Uri(configuration.Settings.WebServiceEndpointAddress);
            return new Uri(endpoint, string.Format("Home/{0}", @this.WebSurveyId));
        }
    }
}
