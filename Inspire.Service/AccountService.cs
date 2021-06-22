namespace Inspire.Services
{
    using System;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.User;

    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository accountRepository;

        public AccountService(IMapper mapper, IRequestData requestData, IAccountRepository accountRepository)
            : base(mapper, requestData)
        {
            this.accountRepository = accountRepository;
        }

        public User GetByUserName(string userName)
        {
            return accountRepository.GetByUserName(userName);
        }

        public Guid LogLoginAction(Guid id, string userEmail)
        {
            return accountRepository.LogLoginAction(id, userEmail, RequestData.Address);
        }
    }
}