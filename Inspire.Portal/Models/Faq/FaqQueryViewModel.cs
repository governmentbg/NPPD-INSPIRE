namespace Inspire.Portal.Models.Faq
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class FaqQueryViewModel
    {
        public Guid? Id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? From { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? To { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Category")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList)]
        public Guid? Category { get; set; }

        public List<KeyValuePair<string, string>> CategoryDataSource { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Status")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList)]
        public Guid? Status { get; set; }

        public List<KeyValuePair<string, string>> StatusDataSource { get; set; }

        public bool? IsValid { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Content")]
        public string SearchWord { get; set; }
    }
}