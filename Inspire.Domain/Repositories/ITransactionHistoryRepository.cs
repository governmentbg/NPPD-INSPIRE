namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface ITransactionHistoryRepository
    {
        List<TransactionHistoryTableModel> Search(TransactionHistoryQueryModel query);

        byte[] MetadataXml(Guid identifier);
    }
}