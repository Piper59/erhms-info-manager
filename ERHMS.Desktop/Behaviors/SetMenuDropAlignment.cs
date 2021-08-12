using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Windows;

namespace ERHMS.Desktop.Behaviors
{
    public class SetMenuDropAlignment : Behavior<DependencyObject>
    {
        public bool Value { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            _ = SystemParameters.MenuDropAlignment;
            FieldInfo field = typeof(SystemParameters)
                .GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            try
            {
                field.SetValue(null, Value);
            }
            catch { }
        }
    }
}
