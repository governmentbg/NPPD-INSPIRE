namespace Inspire.Services
{
    using System;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;

    public class LogService : BaseService, ILogService
    {
        private readonly ILogRepository logRepository;

        public LogService(IMapper mapper, IRequestData requestData, ILogRepository logRepository)
            : base(mapper, requestData)
        {
            this.logRepository = logRepository;
        }

        public void Insert(Guid systemId, string description)
        {
            logRepository.Insert(systemId, RequestData.Address, description);
        }
    }
}