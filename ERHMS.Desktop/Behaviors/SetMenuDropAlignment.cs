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
            try
            {
                if (SystemParameters.MenuDropAlignment != Value)
                {
                    FieldInfo field = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
                    field.SetValue(null, Value);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
            }
        }
    }
}
