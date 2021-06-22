namespace Inspire.Core.Infrastructure.Cache
{
    using System;

    public class NoCacheService : ICacheService
    {
        public bool ContainsKey(string key)
        {
            return false;
        }

        public T GetValue<T>(string key)
        {
            return default(T);
        }

        public void SetValue<T>(string key, T value)
        {
        }

        public void Remove(string key)
        {
        }

        public void Clear()
        {
        }

        public T GetOrSetCache<T>(string key, Func<T> func)
        {
            return func();
        }
    }
}