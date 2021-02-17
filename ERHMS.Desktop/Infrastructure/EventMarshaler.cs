using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ERHMS.Desktop.Infrastructure
{
    public class EventMarshaler : IDisposable
    {
        private static readonly MethodInfo SynchronizationContextSendMethod = typeof(SynchronizationContext).GetMethod(
            nameof(SynchronizationContext.Send),
            new Type[]
            {
                typeof(SendOrPostCallback),
                typeof(object)
            });

        public object Source { get; }
        public EventInfo Event { get; }
        public Delegate MarshaledHandler { get; }

        public EventMarshaler(
            object source,
            EventInfo @event,
            Delegate handler,
            SynchronizationContext synchronizationContext = null)
        {
            Source = source;
            Event = @event;
            if (handler.Method.ReturnType != typeof(void))
            {
                throw new ArgumentException("Handler return type must be void.", nameof(handler));
            }
            if (synchronizationContext == null)
            {
                synchronizationContext = SynchronizationContext.Current;
                if (synchronizationContext == null)
                {
                    throw new ArgumentNullException(nameof(synchronizationContext));
                }
            }
            IReadOnlyCollection<ParameterExpression> parameters = handler.Method.GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
                .ToList();
            // MarshaledHandler = (parameters...) => synchronizationContext.Send(_ => Handler(parameters...), null);
            MarshaledHandler = Expression.Lambda(
                handler.GetType(),
                Expression.Call(
                    Expression.Constant(synchronizationContext),
                    SynchronizationContextSendMethod,
                    Expression.Lambda(
                        typeof(SendOrPostCallback),
                        Expression.Invoke(
                            Expression.Constant(handler),
                            parameters),
                        Expression.Parameter(typeof(object), "_")),
                    Expression.Constant(null)),
                parameters)
                .Compile();
            Event.AddEventHandler(Source, MarshaledHandler);
        }

        public void Dispose()
        {
            Event.RemoveEventHandler(Source, MarshaledHandler);
        }
    }
}
