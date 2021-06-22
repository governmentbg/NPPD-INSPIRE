namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Repository;
    using Inspire.Model.Cms;
    using Inspire.Model.QueryModels;

    public interface ICmsRepository : IRepository
    {
        void Delete(Guid id);

        List<Page> SearchPages(PageQueryModel query, Guid language);

        List<Page> GetParentPages(Guid pageId, Guid language);

        void Upsert(Page page);

        Page GetPage(Guid id, Guid? language = null);

        void ChangePagePosition(Guid pageId, long newPosition, Guid? newParentId);
    }
}
