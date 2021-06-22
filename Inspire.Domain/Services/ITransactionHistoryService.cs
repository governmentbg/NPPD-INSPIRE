namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface ITransactionHistoryService
    {
        List<TransactionHistoryTableModel> Search(TransactionHistoryQueryModel query);

        byte[] MetadataXml(Guid identifier);
    }
}