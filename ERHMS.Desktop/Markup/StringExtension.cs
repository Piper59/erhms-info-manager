using ERHMS.Desktop.Properties;
using System;
using System.Windows.Markup;

namespace ERHMS.Desktop.Markup
{
    [MarkupExtensionReturnType(typeof(string))]
    public class StringExtension : MarkupExtension
    {
        [ConstructorArgument("resourceName")]
        public string ResourceName { get; set; }

        public StringExtension() { }

        public StringExtension(string resourceName)
        {
            ResourceName = resourceName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Strings.ResourceManager.GetString(ResourceName);
        }
    }
}
