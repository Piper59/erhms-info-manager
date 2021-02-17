using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Windows;

namespace ERHMS.Desktop.Behaviors
{
    public class SetMenuDropAlignment : Behavior<DependencyObject>
    {
        private static readonly FieldInfo Field = typeof(SystemParameters)
            .GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

        static SetMenuDropAlignment()
        {
            _ = SystemParameters.MenuDropAlignment;
        }

        public bool Value { get; set; }

        protected override void OnAttached()
        {
            try
            {
                Field.SetValue(null, Value);
            }
            catch { }
        }
    }
}
