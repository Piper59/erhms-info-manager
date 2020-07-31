using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Expression = System.Linq.Expressions.Expression;

namespace ERHMS.Desktop.Behaviors
{
    public class RelayEventBehavior : Behavior<FrameworkElement>
    {
        private class HandlerInfo
        {
            public Type DelegateType { get; }
            public MethodInfo Method { get; }
            public ParameterExpression[] Parameters { get; }

            public HandlerInfo(EventInfo @event)
            {
                DelegateType = @event.EventHandlerType;
                Method = DelegateType.GetMethod(nameof(EventHandler.Invoke));
                Parameters = Method.GetParameters()
                    .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
                    .ToArray();
            }
        }

        private static readonly MethodInfo DispatcherInvokeMethod;

        static RelayEventBehavior()
        {
            DispatcherInvokeMethod = typeof(Dispatcher).GetMethod(nameof(Dispatcher.Invoke), new Type[]
            {
                typeof(Action)
            });
        }

        private EventInfo @event;
        private Delegate relay;

        public string EventName { get; set; }
        public string HandlerName { get; set; }

        protected override void OnAttached()
        {
            AddRelay(AssociatedObject.DataContext);
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
            RemoveRelay(AssociatedObject.DataContext);
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RemoveRelay(e.OldValue);
            AddRelay(e.NewValue);
        }

        private void AddRelay(object dataContext)
        {
            if (dataContext != null)
            {
                @event = dataContext.GetType().GetEvent(EventName);
                HandlerInfo info = new HandlerInfo(@event);
                Delegate handler = Delegate.CreateDelegate(info.DelegateType, AssociatedObject, HandlerName);
                Expression handlerInvocation = Expression.Call(
                    Expression.Constant(handler),
                    info.Method,
                    info.Parameters);
                Expression dispatcherInvocation = Expression.Call(
                    Expression.Constant(Dispatcher),
                    DispatcherInvokeMethod,
                    Expression.Lambda(handlerInvocation));
                relay = Expression.Lambda(info.DelegateType, dispatcherInvocation, info.Parameters).Compile();
                @event.AddEventHandler(dataContext, relay);
            }
        }

        private void RemoveRelay(object dataContext)
        {
            if (dataContext != null)
            {
                @event.RemoveEventHandler(dataContext, relay);
            }
        }
    }
}
