namespace Inspire.Portal.Areas.Admin.Models.Queries
{
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class GroupQueryViewModel
    {
        public long? Id { get; set; }

        [QueryOptions(CssClass = "fifth")]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }
    }
}