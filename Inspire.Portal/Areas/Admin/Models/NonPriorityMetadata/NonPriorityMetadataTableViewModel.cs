namespace Inspire.Portal.Areas.Admin.Models.NonPriorityMetadata
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    [DataContract(Name = "NonPriorityMetadata", Namespace = "")]
    public class NonPriorityMetadataTableViewModel
    {
        [DataMember(Order = 1)]
        public Guid? MetadataGuid { get; set; }

        [DataMember(Order = 2)]
        [Display(Name = "ResourceTitle", ResourceType = typeof(Resource))]
        [TableOptions(ClientHtmlTemplateId = "metadataTitleTemplate")]
        public string MetadataTitle { get; set; }

        [DataMember(Order = 3)]
        [Display(Name = "ResourceTheme", ResourceType = typeof(Resource))]
        public string MetadataTheme { get; set; }

        [DataMember(Order = 4)]
        [Display(ResourceType = typeof(Resource), Name = "NPPDIsValid")]
        [TableOptions(ClientHtmlTemplateId = "isMetadataValidTemplate")]
        public bool? IsMetadataValid { get; set; }

        [DataMember(Order = 5)]
        [Display(ResourceType = typeof(Resource), Name = "IsEUValid")]
        [TableOptions(ClientHtmlTemplateId = "isMetadataEUValidTemplate")]
        public bool? IsMetadataEUValid { get; set; }
    }
}