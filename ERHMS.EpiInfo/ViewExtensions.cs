using Epi;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static bool IsPublished(this View @this)
        {
            return !string.IsNullOrEmpty(@this.WebSurveyId);
        }

        public static string GetUrl(this View @this)
        {
            Configuration configuration = Configuration.GetNewInstance();
            string address = configuration.Settings.WebServiceEndpointAddress;
            int index = address.LastIndexOf('/');
            return string.Format("{0}/Home/{1}", address.Substring(0, index), @this.WebSurveyId);
        }
    }
}
