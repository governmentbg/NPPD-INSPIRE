namespace Inspire.Portal.Models.Poll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class PollQueryViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "Status")]
        [QueryOptions(Type = QueryOptionsAttribute.VisualType.DropDownList, CssClass = "fifth")]
        public Guid? Status { get; set; }

        public List<KeyValuePair<string, string>> StatusDataSource { get; set; }
    }
}