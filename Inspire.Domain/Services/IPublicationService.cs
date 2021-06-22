namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Publication;

    public interface IPublicationService
    {
        Guid Upsert(Publication model);

        Publication Get(Guid id, bool isEdit);

        List<Publication> Search(PublicationQuery query);

        List<Publication> GetVisiblePublicationsByType(PublicationType type);

        void Delete(Guid id);
    }
}