namespace Inspire.Portal.Areas.Admin.Models.TransactionHistory
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;

    public class TransactionHistoryQueryModel
    {
        [Display(ResourceType = typeof(Resource), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Keywords")]
        public string Keyword { get; set; }
    }
}