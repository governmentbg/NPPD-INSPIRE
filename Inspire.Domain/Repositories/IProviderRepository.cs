namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using Inspire.Model.Provider;

    using Inspire.Model.QueryModels;

    public interface IProviderRepository
    {
        Guid Upsert(Provider model);

        Provider Get(Guid id, Guid? languageId);

        IEnumerable<Provider> Search(ProviderQueryModel query);

        void Delete(Guid id);

        void Rearrange(Guid[] ids);
    }
}
