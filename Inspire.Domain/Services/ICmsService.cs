namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Cms;
    using Inspire.Model.QueryModels;

    public interface ICmsService
    {
        List<Page> SearchPages(PageQueryModel query);

        List<Page> GetParentPages(Guid pageId);

        void Delete(Guid id);

        void Upsert(Page page);

        Page GetPage(Guid id, bool isEdit = false);

        void ChangePagePosition(Guid pageId, long newPosition, Guid? newParentId);
    }
}