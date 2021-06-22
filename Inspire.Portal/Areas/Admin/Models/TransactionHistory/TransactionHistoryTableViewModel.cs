namespace Inspire.Portal.Areas.Admin.Models.TransactionHistory
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    using Newtonsoft.Json;

    [DataContract(Name = "TransactionHistory", Namespace = "")]
    public class TransactionHistoryTableViewModel
    {
        public Guid MetadataHistoryId { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Identitier")]
        [TableOptions(ClientHtmlTemplateId = "metadataIdentifierTemplate")]
        public string MetadataIdentifier { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        [TableOptions(ClientHtmlTemplateId = "metadataTitleTemplate")]
        public string MetadataTitle { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "CreateDate")]
        [TableOptions(Format = "G")]
        public DateTime CreateDate { get; set; }

        [DataMember(Order = 4)]
        [Display(ResourceType = typeof(Resource), Name = "ChangeDate")]
        [TableOptions(Format = "G")]
        public DateTime? ChangeDate { get; set; }

        [DataMember(Order = 5)]
        [Display(ResourceType = typeof(Resource), Name = "OperationType")]
        public string OperationType { get; set; }

        [DataMember(Order = 6)]
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string UserName { get; set; }

        [DataMember(Order = 7)]
        [Display(ResourceType = typeof(Resource), Name = "Organization")]
        public string Organization { get; set; }

        [DataMember(Order = 8)]
        [Display(ResourceType = typeof(Resource), Name = "Schema")]
        public string Schema { get; set; }

        [JsonIgnore]
        public bool IsHarvested { get; set; }

        [DataMember(Order = 9)]
        [Display(ResourceType = typeof(Resource), Name = "IsHarvested")]
        public string Harvested => IsHarvested ? Resource.Yes : Resource.No;
    }
}