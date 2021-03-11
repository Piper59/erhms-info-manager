using ERHMS.Common;
using Microsoft.Xaml.Behaviors;
using System;
using System.Reflection;
using System.Windows;

namespace ERHMS.Desktop.Behaviors
{
    public class MarshalDataContextEvent : Behavior<FrameworkElement>
    {
        private EventMarshaler marshaler;

        public string EventName { get; set; }
        public string HandlerName { get; set; }

        protected override void OnAttached()
        {
            CreateMarshaler();
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
            DestroyMarshaler();
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DestroyMarshaler();
            CreateMarshaler();
        }

        private bool CreateMarshaler()
        {
            if (AssociatedObject.DataContext == null)
            {
                return false;
            }
            EventInfo @event = AssociatedObject.DataContext.GetType().GetEvent(EventName);
            Delegate handler = Delegate.CreateDelegate(@event.EventHandlerType, AssociatedObject, HandlerName);
            marshaler = new EventMarshaler(AssociatedObject.DataContext, @event, handler);
            return true;
        }

        private bool DestroyMarshaler()
        {
            if (marshaler == null)
            {
                return false;
            }
            marshaler.Dispose();
            marshaler = null;
            return true;
        }
    }
}
