using ERHMS.Common.Logging;
using Microsoft.Xaml.Behaviors;
using System;
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
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
            }
        }
    }
}
