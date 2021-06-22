namespace Inspire.Portal.Areas.Admin.Models.PriorityMetadata
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    [DataContract(Name = "PriorityMetadata", Namespace = "")]
    public class PriorityMetadataTableViewModel
    {
        [DataMember(Order = 1)]
        [Display(Name = "IdCode", ResourceType = typeof(Resource))]
        public string DirectiveCode { get; set; }

        [DataMember(Order = 2)]
        [Display(Name = "PriorityMetadata", ResourceType = typeof(Resource))]
        public string DirectiveTitle { get; set; }

        [DataMember(Order = 3)]
        [Display(Name = "Organization", ResourceType = typeof(Resource))]
        public string DirectiveOrganization { get; set; }

        [DataMember(Order = 4)]
        [Display(Name = "OrganizationInNPPD", ResourceType = typeof(Resource))]
        public string Organization { get; set; }

        [DataMember(Order = 5)]
        [Display(Name = "NPPDInspirePriorityDataSet", ResourceType = typeof(Resource))]
        public string InspirePriorityDataSet { get; set; }

        [DataMember(Order = 6)]
        public Guid? MetadataGuid { get; set; }

        [DataMember(Order = 7)]
        [Display(Name = "ResourceTitle", ResourceType = typeof(Resource))]
        [TableOptions(ClientHtmlTemplateId = "metadataTitleTemplate")]
        public string MetadataTitle { get; set; }

        [DataMember(Order = 8)]
        [Display(ResourceType = typeof(Resource), Name = "NPPDIsValid")]
        [TableOptions(ClientHtmlTemplateId = "isMetadataValidTemplate")]
        public bool? IsMetadataValid { get; set; }

        [DataMember(Order = 9)]
        [Display(ResourceType = typeof(Resource), Name = "IsEUValid")]
        [TableOptions(ClientHtmlTemplateId = "isMetadataEUValidTemplate")]
        public bool? IsMetadataEUValid { get; set; }
    }
}