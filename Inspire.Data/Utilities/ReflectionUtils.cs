namespace Inspire.Data.Utilities
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    public class ReflectionUtils
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyInfoCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] GetPropertyInfoCache(Type type)
        {
            if (PropertyInfoCache.ContainsKey(type))
            {
                return PropertyInfoCache[type];
            }

            var properties = type.GetProperties();
            PropertyInfoCache.TryAdd(type, properties);

            return properties;
        }
    }
}