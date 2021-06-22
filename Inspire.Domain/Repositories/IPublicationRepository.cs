namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Publication;

    public interface IPublicationRepository
    {
        Guid Upsert(Publication model);

        Publication Get(Guid id, Guid? language = null);

        List<Publication> Search(PublicationQuery query);

        void Delete(Guid id);
    }
}