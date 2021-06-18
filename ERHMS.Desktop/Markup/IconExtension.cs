using System;
using System.Windows.Markup;

namespace ERHMS.Desktop.Markup
{
    [MarkupExtensionReturnType(typeof(string))]
    public class IconExtension : MarkupExtension
    {
        [ConstructorArgument("name")]
        public string Name { get; set; }

        public IconExtension() { }

        public IconExtension(string name)
            : this()
        {
            Name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Icons.GetInstance(Name);
        }
    }
}
