using System;
using System.Collections.Generic;

namespace TapVerse.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (service == null)
            {
                Services.Remove(type);
                return;
            }

            Services[type] = service;
        }

        public static bool TryResolve<T>(out T service)
        {
            if (Services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = default;
            return false;
        }

        public static T Resolve<T>()
        {
            if (TryResolve<T>(out var service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service {typeof(T).Name} has not been registered");
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}
