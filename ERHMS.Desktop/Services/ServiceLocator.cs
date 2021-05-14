using System;
using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public static class ServiceLocator
    {
        private static readonly IDictionary<Type, Delegate> factoriesByType = new Dictionary<Type, Delegate>();

        public static void Install<TService>(Func<TService> factory)
        {
            factoriesByType[typeof(TService)] = factory;
        }

        public static TService Resolve<TService>()
        {
            Func<TService> factory = (Func<TService>)factoriesByType[typeof(TService)];
            return factory();
        }
    }
}
