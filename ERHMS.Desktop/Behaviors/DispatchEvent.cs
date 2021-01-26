using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Expression = System.Linq.Expressions.Expression;

namespace ERHMS.Desktop.Behaviors
{
    public class DispatchEvent : Behavior<FrameworkElement>
    {
        private class Relay : IDisposable
        {
            public object Source { get; }
            public DispatcherObject Target { get; }
            public EventInfo Event { get; }
            public Delegate Handler { get; }
            public Delegate DispatchedHandler { get; }

            public Relay(FrameworkElement element, string eventName, string handlerName)
            {
                Source = element.DataContext;
                Target = element;
                Event = Source.GetType().GetEvent(eventName);
                Handler = Delegate.CreateDelegate(Event.EventHandlerType, Target, handlerName);
                ICollection<ParameterExpression> parameters = Handler.Method.GetParameters()
                    .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
                    .ToList();
                DispatchedHandler = Expression.Lambda(
                    Event.EventHandlerType,
                    Expression.Call(
                        Expression.Constant(Target.Dispatcher),
                        typeof(Dispatcher).GetMethod(nameof(Dispatcher.Invoke), new Type[] { typeof(Action) }),
                        Expression.Lambda(
                            Expression.Invoke(
                                Expression.Constant(Handler),
                                parameters))),
                    parameters)
                    .Compile();
                Event.AddEventHandler(Source, DispatchedHandler);
            }

            public void Dispose()
            {
                Event.RemoveEventHandler(Source, DispatchedHandler);
            }
        }

        private Relay relay;

        public string EventName { get; set; }
        public string HandlerName { get; set; }

        protected override void OnAttached()
        {
            CreateRelay();
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
            DestroyRelay();
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DestroyRelay();
            CreateRelay();
        }

        private bool CreateRelay()
        {
            if (relay != null)
            {
                throw new InvalidOperationException("Relay already exists.");
            }
            if (AssociatedObject.DataContext == null)
            {
                return false;
            }
            else
            {
                relay = new Relay(AssociatedObject, EventName, HandlerName);
                return true;
            }
        }

        private bool DestroyRelay()
        {
            if (relay == null)
            {
                return false;
            }
            else
            {
                relay.Dispose();
                relay = null;
                return true;
            }
        }
    }
}
