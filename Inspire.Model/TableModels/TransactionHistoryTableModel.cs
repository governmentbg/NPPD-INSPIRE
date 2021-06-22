namespace Inspire.Model.TableModels
{
    using System;

    public class TransactionHistoryTableModel
    {
        public Guid MetadataHistoryId { get; set; }

        public string MetadataIdentifier { get; set; }

        public string MetadataTitle { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ChangeDate { get; set; }

        public string OperationType { get; set; }

        public string UserName { get; set; }

        public string User { get; set; }

        public string Organization { get; set; }

        public string Schema { get; set; }

        public bool IsHarvested { get; set; }
    }
}