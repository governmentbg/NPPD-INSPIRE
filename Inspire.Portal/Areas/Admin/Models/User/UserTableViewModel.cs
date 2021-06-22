namespace Inspire.Portal.Areas.Admin.Models.User
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    using Newtonsoft.Json;

    [DataContract(Name = "User", Namespace = "")]
    public class UserTableViewModel
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid StatusId { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        [TableOptions(ClientHtmlTemplateId = "userInfoTemplate")]
        public string UserName { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "Phone")]
        public string Phone { get; set; }

        [DataMember(Order = 5)]
        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public string Status { get; set; }

        [DataMember(Order = 6)]
        [Display(ResourceType = typeof(Resource), Name = "Roles")]
        public string Roles { get; set; }

        [DataMember(Order = 7)]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        public string Email { get; internal set; }

        [DataMember(Order = 8)]
        [Display(ResourceType = typeof(Resource), Name = "Organization")]
        public string Organisation { get; set; }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "userActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}