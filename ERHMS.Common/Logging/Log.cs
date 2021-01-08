using log4net;
using System;

namespace ERHMS.Common.Logging
{
    public static class Log
    {
        private static readonly Lazy<ILog> instance = new Lazy<ILog>(GetInstance);
        public static ILog Instance => instance.Value;

        private static EventHandler initializing;
        public static event EventHandler Initializing
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

        private static void OnInitializing(EventArgs e) => initializing?.Invoke(null, e);
        private static void OnInitializing() => OnInitializing(EventArgs.Empty);

        private static ILog GetInstance()
        {
            OnInitializing();
            return LogManager.GetLogger(nameof(ERHMS));
        }
    }
}
