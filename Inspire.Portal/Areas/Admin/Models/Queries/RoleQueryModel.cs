namespace Inspire.Portal.Areas.Admin.Models.Queries
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class RoleQueryModel
    {
        public Guid? Id { get; set; }

        [QueryOptions(CssClass = "triple")]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }
    }
}