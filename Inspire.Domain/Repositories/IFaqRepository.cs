namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Faq;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IFaqRepository
    {
        // FAQ functionalities
        List<FaqTableModel> Search(FaqQueryModel query);

        void Upsert(Faq model);

        void Delete(Guid id);

        Faq GetFaq(Guid id, Guid? languageId);

        // FAQ's Category functionalities
        Guid UpsertFaqCategory(FaqCategory category);

        FaqCategory GetFaqCategory(Guid id, Guid? languageId);

        List<FaqCategory> SearchFaqCategories(Guid languageId, bool isValid = true);

        void DeleteFaqCategory(Guid id, Guid? newCategoryId);

        void Rearrange(Guid[] ids);
    }
}
