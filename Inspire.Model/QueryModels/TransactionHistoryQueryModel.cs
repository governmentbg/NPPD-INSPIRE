namespace Inspire.Model.QueryModels
{
    using System;

    public class TransactionHistoryQueryModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string Keyword { get; set; }
    }
}