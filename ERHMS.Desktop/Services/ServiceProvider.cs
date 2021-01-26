using System;
using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public static class ServiceProvider
    {
        private static readonly IDictionary<Type, Delegate> factories = new Dictionary<Type, Delegate>();

        public static void Install<TService>(Func<TService> factory)
        {
            factories.Add(typeof(TService), factory);
        }

        public static TService Resolve<TService>()
        {
            Func<TService> factory = (Func<TService>)factories[typeof(TService)];
            return factory();
        }
    }
}
