namespace Inspire.Portal.Areas.Admin.Models.Role
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    [DataContract(Name = "Role", Namespace = "")]
    public class RoleTableViewModel
    {
        public Guid? Id { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "Rights")]
        public string ActivitiesString { get; set; }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "roleActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}