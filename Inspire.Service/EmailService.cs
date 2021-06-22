namespace Inspire.Services
{
    using System;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Email;

    public class EmailService : BaseService, IEmailService
    {
        private readonly IEmailRepository emailRepository;

        public EmailService(IMapper mapper, IRequestData requestData, IEmailRepository emailRepository)
            : base(mapper, requestData)
        {
            this.emailRepository = emailRepository;
        }

        public Guid Insert(Email model)
        {
            return emailRepository.Insert(model);
        }

        public byte[] GetMailContentById(Guid mailContentId)
        {
            return emailRepository.GetMailContentById(mailContentId);
        }
    }
}