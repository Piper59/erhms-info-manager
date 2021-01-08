using log4net;
using log4net.Repository.Hierarchy;
using System;

namespace ERHMS.Common.Logging
{
    public static class Log
    {
        private static readonly Lazy<ILog> instance = new Lazy<ILog>(GetInstance);
        public static ILog Instance => instance.Value;

        private static EventHandler<InitializingEventArgs> initializing;
        public static event EventHandler<InitializingEventArgs> Initializing
        {
            add
            {
                if (instance.IsValueCreated)
                {
                    throw new InvalidOperationException("Log has already been initialized.");
                }
                initializing += value;
            }
            remove
            {
                initializing -= value;
            }
        }

        private static void OnInitializing(InitializingEventArgs e)
        {
            initializing?.Invoke(null, e);
            e.Hierarchy.Configured = true;
        }

        private static void OnInitializing(Hierarchy hierarchy) => OnInitializing(new InitializingEventArgs(hierarchy));

        private static ILog GetInstance()
        {
            OnInitializing((Hierarchy)LogManager.GetRepository());
            return LogManager.GetLogger(nameof(ERHMS));
        }
    }
}
