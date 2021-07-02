using System;
using System.Windows.Markup;

namespace ERHMS.Desktop.Markup
{
    [MarkupExtensionReturnType(typeof(string))]
    public class IconExtension : MarkupExtension
    {
        [ConstructorArgument("iconName")]
        public string IconName { get; set; }

        public IconExtension() { }

        public IconExtension(string iconName)
            : this()
        {
            IconName = iconName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Icons.GetInstance(IconName);
        }
    }
}
