namespace Inspire.Core.Infrastructure.Cache
{
    using System;

    public interface ICacheService
    {
        bool ContainsKey(string key);

        T GetValue<T>(string key);

        void SetValue<T>(string key, T value);

        void Remove(string key);

        void Clear();

        T GetOrSetCache<T>(string key, Func<T> func);
    }
}