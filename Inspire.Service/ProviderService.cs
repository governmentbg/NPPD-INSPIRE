namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Attachment;
    using Inspire.Model.Provider;
    using Inspire.Model.QueryModels;
    using Inspire.Utilities.Enums;

    public class ProviderService : BaseService, IProviderService
    {
        private readonly IProviderRepository providerRepository;
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IStorageService storageService;

        public ProviderService(IMapper mapper, IRequestData requestData, IProviderRepository providerRepository, IAttachmentRepository attachmentRepository, IStorageService storageService)
            : base(mapper, requestData)
        {
            this.providerRepository = providerRepository;
            this.attachmentRepository = attachmentRepository;
            this.storageService = storageService;
        }

        public void Delete(Guid id)
        {
            providerRepository.Delete(id);
        }

        public Provider Get(Guid id, bool isEdit)
        {
            var provider = providerRepository.Get(id, isEdit ? null : new Guid?(RequestData.LanguageId));
            if (provider != null)
            {
                provider.MainPicture = attachmentRepository.GetFiles(id, ObjectType.Provider).FirstOrDefault();
            }

            return provider;
        }

        public void Rearrange(Guid[] ids)
        {
            providerRepository.Rearrange(ids);
        }

        public IEnumerable<Provider> Search(ProviderQueryModel query)
        {
            query.LanguageId = RequestData.LanguageId;
            return providerRepository.Search(query);
        }

        public Guid Upsert(Provider model)
        {
            model.Id = providerRepository.Upsert(model);
            storageService.Save(new List<Attachment>() { model.MainPicture }, model.Id.Value, ObjectType.Provider);
            return model.Id.Value;
        }
    }
}
