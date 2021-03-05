using System;
using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public static class ServiceProvider
    {
        private static readonly IDictionary<Type, object> servicesByType = new Dictionary<Type, object>();

        public static void Install<TService>(TService service)
        {
            servicesByType[typeof(TService)] = service;
        }

        public static TService Resolve<TService>()
        {
            return (TService)servicesByType[typeof(TService)];
        }
    }
}
