using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ERHMS.Common
{
    public class EventMarshaler : IDisposable
    {
        private static readonly MethodInfo synchronizationContextSendMethod = typeof(SynchronizationContext).GetMethod(
            nameof(SynchronizationContext.Send),
            new Type[]
            {
                typeof(SendOrPostCallback),
                typeof(object)
            });

        private Delegate marshaledHandler;

        public object Source { get; }
        public EventInfo Event { get; }
        public Delegate Handler { get; }
        public SynchronizationContext SynchronizationContext { get; }

        public EventMarshaler(
            object source,
            EventInfo @event,
            Delegate handler,
            SynchronizationContext synchronizationContext)
        {
            Source = source;
            Event = @event;
            Handler = handler;
            SynchronizationContext = synchronizationContext;
        }

        public EventMarshaler(object source, EventInfo @event, Delegate handler)
            : this(source, @event, handler, SynchronizationContext.Current) { }

        public void Attach()
        {
            IReadOnlyCollection<ParameterExpression> parameters = Handler.Method.GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
                .ToList();
            // marshaledHandler = (parameters...) => synchronizationContext.Send(_ => Handler(parameters...), null);
            marshaledHandler = Expression.Lambda(
                Event.EventHandlerType,
                Expression.Call(
                    Expression.Constant(SynchronizationContext),
                    synchronizationContextSendMethod,
                    Expression.Lambda(
                        typeof(SendOrPostCallback),
                        Expression.Invoke(
                            Expression.Constant(Handler),
                            parameters),
                        Expression.Parameter(typeof(object), "_")),
                    Expression.Constant(null)),
                parameters)
                .Compile();
            Event.AddEventHandler(Source, marshaledHandler);
        }

        public void Detach()
        {
            Event.RemoveEventHandler(Source, marshaledHandler);
        }

        public void Dispose()
        {
            Detach();
        }
    }
}
