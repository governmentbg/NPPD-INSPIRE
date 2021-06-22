namespace Inspire.Portal.Models.Publication
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class PublicationQueryModel
    {
        public Guid? Id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Type")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList)]
        public Guid? TypeId { get; set; }

        public List<KeyValuePair<string, string>> TypeIdDataSource { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? StartDateTo { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VisibilityInPublicPart")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList)]
        public bool? IsVisible { get; set; }

        public List<KeyValuePair<string, string>> IsVisibleDataSource { get; set; }
    }
}