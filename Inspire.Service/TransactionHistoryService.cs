namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public class TransactionHistoryService : BaseService, ITransactionHistoryService
    {
        private readonly ITransactionHistoryRepository transactionHistoryRepository;

        public TransactionHistoryService(IMapper mapper, IRequestData requestData, ITransactionHistoryRepository transactionHistoryRepository)
            : base(mapper, requestData)
        {
            this.transactionHistoryRepository = transactionHistoryRepository;
        }

        public List<TransactionHistoryTableModel> Search(TransactionHistoryQueryModel query)
        {
            return transactionHistoryRepository.Search(query);
        }

        public byte[] MetadataXml(Guid identifier)
        {
            return transactionHistoryRepository.MetadataXml(identifier);
        }
    }
}
