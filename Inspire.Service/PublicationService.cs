namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Publication;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Extensions;

    public class PublicationService : BaseService, IPublicationService
    {
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IPublicationRepository publicationRepository;
        private readonly IStorageService storageService;

        public PublicationService(
            IMapper mapper,
            IRequestData requestData,
            IPublicationRepository publicationRepository,
            IStorageService storageService,
            IAttachmentRepository attachmentRepository)
            : base(mapper, requestData)
        {
            this.publicationRepository = publicationRepository;
            this.storageService = storageService;
            this.attachmentRepository = attachmentRepository;
        }

        public Guid Upsert(Publication model)
        {
            if (model.Pictures.IsNotNullOrEmpty() && model.Pictures.SingleOrDefault(item => item.IsMain) == null)
            {
                model.Pictures.First().IsMain = true;
            }

            model.Id = publicationRepository.Upsert(model);
            storageService.Save(model.Pictures, model.Id.Value, ObjectType.Publication);
            return model.Id.Value;
        }

        public Publication Get(Guid id, bool isEdit)
        {
            var publication = publicationRepository.Get(
                id,
                isEdit
                    ? null
                    : new Guid?(RequestData.LanguageId));

            if (publication != null)
            {
                publication.Pictures = attachmentRepository.GetFiles(id, ObjectType.Publication);
            }

            return publication;
        }

        public List<Publication> Search(PublicationQuery query)
        {
            query.LanguageId = RequestData.LanguageId;
            return publicationRepository.Search(query);
        }

        public List<Publication> GetVisiblePublicationsByType(PublicationType type)
        {
            var result = publicationRepository
                .Search(
                    new PublicationQuery
                    {
                        IsVisible = true,
                        StartDateTo = type == PublicationType.News ? DateTime.Now.Date.AddDays(1) : default(DateTime?),
                        LanguageId = RequestData.LanguageId,
                        TypeId = EnumHelper.PublicationTypes[type]
                    });
            if (result.IsNotNullOrEmpty())
            {
                foreach (var publication in result)
                {
                    publication.Pictures = attachmentRepository.GetFiles(publication.Id.Value, ObjectType.Publication);
                }
            }

            return result;
        }

        public void Delete(Guid id)
        {
            publicationRepository.Delete(id);
        }
    }
}