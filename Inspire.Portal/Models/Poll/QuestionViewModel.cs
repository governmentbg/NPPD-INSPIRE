namespace Inspire.Portal.Models.Poll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Extensions;

    public class QuestionViewModel : BaseDbModel, IValidatableObject
    {
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Title => Titles.GetValueForCurrentCulture();

        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public string Description => QuestionDescriptions.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public SortedDictionary<string, string> Titles { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public SortedDictionary<string, string> QuestionDescriptions { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public Nomenclature Type { get; set; }

        [Display(Name = "Options", ResourceType = typeof(Resource))]
        public List<OptionViewModel> Options { get; set; }

        [Display(Name = "RequiredFlag", ResourceType = typeof(Resource))]
        public bool Mandatory { get; set; }

        public string RadioBtn { get; set; }

        public Guid? PollId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Options.IsNullOrEmpty())
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.Options),
                    new[] { "Options" });
            }
        }
    }
}