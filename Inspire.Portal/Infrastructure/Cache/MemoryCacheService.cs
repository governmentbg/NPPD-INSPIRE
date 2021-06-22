namespace Inspire.Portal.Infrastructure.Cache
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Portal.Utilities;

    public class MemoryCacheService : ICacheService
    {
        private static readonly object CacheLockObject = new object();

        private readonly CacheItemPolicy cachePolicy = ConfigurationReader.CacheExpiration > 0
            ? new CacheItemPolicy
              {
                  AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(ConfigurationReader.CacheExpiration)
              }
            : null;

        public bool ContainsKey(string key)
        {
            return cachePolicy != null && MemoryCache.Default.Contains(key);
        }

        public T GetValue<T>(string key)
        {
            return (T)MemoryCache.Default.Get(key);
        }

        public void SetValue<T>(string key, T value)
        {
            lock (CacheLockObject)
            {
                if (cachePolicy == null)
                {
                    return;
                }

                MemoryCache.Default.Set(key, value, cachePolicy);
            }
        }

        public void Remove(string key)
        {
            lock (CacheLockObject)
            {
                if (!MemoryCache.Default.Contains(key))
                {
                    return;
                }

                MemoryCache.Default.Remove(key);
            }
        }

        public void Clear()
        {
            lock (CacheLockObject)
            {
                ObjectCache cache = MemoryCache.Default;
                var cacheKeys = cache.Select(kvp => kvp.Key).ToList();
                foreach (var cacheKey in cacheKeys)
                {
                    cache.Remove(cacheKey);
                }
            }
        }

        public T GetOrSetCache<T>(string key, Func<T> func)
        {
            T data;
            if (!ContainsKey(key))
            {
                data = func();
                SetValue(key, data);
            }
            else
            {
                data = GetValue<T>(key);
            }

            return data;
        }
    }
}