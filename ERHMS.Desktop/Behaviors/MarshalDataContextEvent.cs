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
            AttachMarshaler();
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
            DetachMarshaler();
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DetachMarshaler();
            AttachMarshaler();
        }

        private bool AttachMarshaler()
        {
            if (AssociatedObject.DataContext == null)
            {
                return false;
            }
            EventInfo @event = AssociatedObject.DataContext.GetType().GetEvent(EventName);
            Delegate handler = Delegate.CreateDelegate(@event.EventHandlerType, AssociatedObject, HandlerName);
            marshaler = new EventMarshaler(AssociatedObject.DataContext, @event, handler);
            marshaler.Attach();
            return true;
        }

        private bool DetachMarshaler()
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
