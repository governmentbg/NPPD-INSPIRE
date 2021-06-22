namespace Inspire.Portal.Areas.Admin.Models.Group
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    [DataContract(Name = "Group", Namespace = "")]
    public class GroupTableViewModel
    {
        public Guid? Id { get; set; }

        public long? GeoNetworkId { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        [TableOptions(ClientHtmlTemplateId = "groupInfoTemplate")]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "Bulstat")]
        public string Bulstat { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        public string Email { get; set; }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "groupActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}