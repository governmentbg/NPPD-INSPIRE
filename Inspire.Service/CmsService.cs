namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Cms;
    using Inspire.Model.QueryModels;

    public class CmsService : BaseService, ICmsService
    {
        private readonly ICmsRepository cmsRepository;

        public CmsService(IMapper mapper, IRequestData requestData, ICmsRepository cmsRepository)
            : base(mapper, requestData)
        {
            this.cmsRepository = cmsRepository;
        }

        public List<Page> GetParentPages(Guid pageId)
        {
            return cmsRepository.GetParentPages(pageId, RequestData.LanguageId);
        }

        public void Delete(Guid id)
        {
            cmsRepository.Delete(id);
        }

        public Page GetPage(Guid id, bool isEdit = false)
        {
            return cmsRepository.GetPage(
                id,
                language: isEdit
                ? null
                : new Guid?(RequestData.LanguageId));
        }

        public void ChangePagePosition(Guid pageId, long newPosition, Guid? newParentId)
        {
            cmsRepository.ChangePagePosition(pageId, newPosition, newParentId);
        }

        public List<Page> SearchPages(PageQueryModel query)
        {
            return cmsRepository.SearchPages(query, RequestData.LanguageId);
        }

        public void Upsert(Page page)
        {
            cmsRepository.Upsert(page);
        }
    }
}
