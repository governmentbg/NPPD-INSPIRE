namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Faq;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IFaqService
    {
        // FAQ functionalities
        List<FaqTableModel> Search(FaqQueryModel query);

        void Upsert(Faq model);

        void Delete(Guid id);

        Faq GetFaq(Guid id, bool isEdit);

        // FAQ's Category functionalities
        Guid UpsertFaqCategory(FaqCategory category);

        FaqCategory GetFaqCategory(Guid id, bool isEdit);

        List<FaqCategory> SearchFaqCategories(bool isValid = true);

        void DeleteFaqCategory(Guid id, Guid? newCategoryId);

        void Rearrange(Guid[] ids);
    }
}
