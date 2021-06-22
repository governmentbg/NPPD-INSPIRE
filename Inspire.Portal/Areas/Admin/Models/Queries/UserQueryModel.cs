namespace Inspire.Portal.Areas.Admin.Models.Queries
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class UserQueryModel
    {
        public Guid? Id { get; set; }

        [QueryOptions(CssClass = "fifth")]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }

        [QueryOptions(CssClass = "fifth")]
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string UserName { get; set; }

        [QueryOptions(CssClass = "fifth")]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Status")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList, CssClass = "fifth")]
        public Guid? Status { get; set; }

        public List<KeyValuePair<string, string>> StatusDataSource { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Organization")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList, CssClass = "fifth")]
        public Guid? Organisation { get; set; }

        public List<KeyValuePair<string, string>> OrganisationDataSource { get; set; }
    }
}