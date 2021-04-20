using System.Reflection;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public static class MenuDropAlignment
    {
        private static readonly FieldInfo field = typeof(SystemParameters)
            .GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

        static MenuDropAlignment()
        {
            _ = SystemParameters.MenuDropAlignment;
        }

        public static bool Value
        {
            get { return SystemParameters.MenuDropAlignment; }
            set { field.SetValue(null, value); }
        }
    }
}
