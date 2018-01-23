using ERHMS.Presentation.Services;
using System.Reflection;
using System.Windows;

namespace ERHMS.Presentation
{
    public class StringService : IStringService
    {
        public string AppBareTitle
        {
            get { return "ERHMS Info Manager"; }
        }

        public string AppTitle
        {
            get { return AppBareTitle + "\u2122"; }
        }

        public string GetStarted
        {
            get
            {
                return string.Join(" ", new string[]
                {
                    "To get started, please create or open a data source.",
                    "Click Create or Add > New on the Data Sources screen."
                });
            }
        }

        public StringService() { }

        public StringService(ResourceDictionary resources)
        {
            foreach (PropertyInfo property in typeof(IStringService).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                resources.Add(property.Name, property.GetValue(this, null));
            }
        }
    }
}
