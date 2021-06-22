namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;

    using Action = Inspire.Model.History.Action;

    public class HistoryService : BaseService, IHistoryService
    {
        private readonly IHistoryRepository historyRepository;

        public HistoryService(IMapper mapper, IRequestData requestData, IHistoryRepository historyRepository)
            : base(mapper, requestData)
        {
            this.historyRepository = historyRepository;
        }

        public List<Action> GetObjectHistory(Guid? objectId, Guid objectType)
        {
            return historyRepository.GetObjectHistory(objectId, objectType, RequestData.LanguageId);
        }
    }
}