namespace Inspire.Portal.Models.Poll
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class OptionViewModel : BaseDbModel
    {
        public string Value => Values.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Value")]
        public SortedDictionary<string, string> Values { get; set; }

        public bool IsSelected { get; set; }
    }
}