namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Faq;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public class FaqService : BaseService, IFaqService
    {
        private readonly IFaqRepository faqRepository;

        public FaqService(IMapper mapper, IRequestData requestData, IFaqRepository faqRepository)
            : base(mapper, requestData)
        {
            this.faqRepository = faqRepository;
        }

        public void Delete(Guid id)
        {
            faqRepository.Delete(id);
        }

        public void DeleteFaqCategory(Guid id, Guid? newCategoryId)
        {
            faqRepository.DeleteFaqCategory(id, newCategoryId);
        }

        public void Rearrange(Guid[] ids)
        {
            faqRepository.Rearrange(ids);
        }

        public Faq GetFaq(Guid id, bool isEdit)
        {
            return faqRepository.GetFaq(id, isEdit ? null : new Guid?(RequestData.LanguageId));
        }

        public FaqCategory GetFaqCategory(Guid id, bool isEdit = true)
        {
            return faqRepository.GetFaqCategory(id, isEdit ? null : new Guid?(RequestData.LanguageId));
        }

        public List<FaqTableModel> Search(FaqQueryModel query)
        {
            query.LanguageId = RequestData.LanguageId;
            return faqRepository.Search(query);
        }

        public List<FaqCategory> SearchFaqCategories(bool isValid = true)
        {
            return faqRepository.SearchFaqCategories(RequestData.LanguageId, isValid);
        }

        public void Upsert(Faq model)
        {
            faqRepository.Upsert(model);
        }

        public Guid UpsertFaqCategory(FaqCategory category)
        {
            return faqRepository.UpsertFaqCategory(category);
        }
    }
}