namespace Inspire.Portal.Services.SessionStorageService
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.SessionState;

    using Inspire.Model.Base;
    using Inspire.Utilities.Extensions;

    public class SessionStorageService : ISessionStorageService
    {
        private readonly HttpSessionState session = HttpContext.Current?.Session;

        public T Get<T>(string uniqueId)
            where T : IModel
        {
            if (uniqueId.IsNullOrEmpty())
            {
                return default(T);
            }

            var items = GetItems<T>();
            items.TryGetValue(uniqueId, out var item);
            return item;
        }

        public void Upsert<T>(T model)
            where T : IModel
        {
            if (model == null)
            {
                return;
            }

            Upsert<T>(new[] { model });
        }

        public void Upsert<T>(IEnumerable<T> items)
            where T : IModel
        {
            if (items.IsNullOrEmpty())
            {
                return;
            }

            var collection = GetItems<T>();
            foreach (var item in items)
            {
                collection[item.UniqueId] = item;
            }
        }

        public void Remove<T>(string uniqueId)
            where T : IModel
        {
            if (uniqueId.IsNullOrEmpty())
            {
                return;
            }

            var items = GetItems<T>();
            if (items.ContainsKey(uniqueId))
            {
                items.Remove(uniqueId);
            }
        }

        public void Remove<T>(T model)
            where T : IModel
        {
            if (model == null)
            {
                return;
            }

            Remove<T>(new[] { model });
        }

        public void Remove<T>(IEnumerable<T> items)
            where T : IModel
        {
            if (items.IsNullOrEmpty())
            {
                return;
            }

            var collection = GetItems<T>();
            foreach (var item in items)
            {
                if (collection.ContainsKey(item.UniqueId))
                {
                    collection.Remove(item.UniqueId);
                }
            }
        }

        public void Clear<T>()
            where T : IModel
        {
            GetItems<T>().Clear();
        }

        private IDictionary<string, T> GetItems<T>()
            where T : IModel
        {
            var sessionKey = typeof(T).Name;
            if (!(session[sessionKey] is IDictionary<string, T>))
            {
                session[sessionKey] = new Dictionary<string, T>();
            }

            return (IDictionary<string, T>)session[sessionKey];
        }
    }
}