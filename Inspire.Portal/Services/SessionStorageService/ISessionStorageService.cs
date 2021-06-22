namespace Inspire.Portal.Services.SessionStorageService
{
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public interface ISessionStorageService
    {
        T Get<T>(string uniqueId)
            where T : IModel;

        void Upsert<T>(T model)
            where T : IModel;

        void Upsert<T>(IEnumerable<T> items)
            where T : IModel;

        void Remove<T>(string uniqueId)
            where T : IModel;

        void Remove<T>(T model)
            where T : IModel;

        void Remove<T>(IEnumerable<T> items)
            where T : IModel;

        void Clear<T>()
            where T : IModel;
    }
}