namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using Inspire.Model.Provider;
    using Inspire.Model.QueryModels;

    public interface IProviderService
    {
        Guid Upsert(Provider model);

        Provider Get(Guid id, bool isEdit);

        IEnumerable<Provider> Search(ProviderQueryModel query);

        void Delete(Guid id);

        void Rearrange(Guid[] ids);
    }
}
