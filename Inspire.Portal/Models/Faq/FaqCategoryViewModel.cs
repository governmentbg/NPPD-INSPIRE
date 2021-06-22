namespace Inspire.Portal.Models.Faq
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class FaqCategoryViewModel : BaseDbModel
    {
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Name => Names.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public SortedDictionary<string, string> Names { get; set; }
    }
}