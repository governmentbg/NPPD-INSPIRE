namespace Inspire.Portal.Models.Poll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Extensions;

    public class PollViewModel : BaseDbModel, IValidatableObject
    {
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Title => Titles.GetValueForCurrentCulture();

        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public string Description => Descriptions.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public SortedDictionary<string, string> Titles { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public SortedDictionary<string, string> Descriptions { get; set; }

        [Display(Name = "Questions", ResourceType = typeof(Resource))]
        public List<QuestionViewModel> Questions { get; set; }

        [Display(Name = "From", ResourceType = typeof(Resource))]
        public DateTime? ValidFrom { get; set; }

        [Display(Name = "To", ResourceType = typeof(Resource))]
        public DateTime? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ValidFrom.HasValue)
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.From),
                    new[] { "ValidFrom" });
            }

            if (!ValidTo.HasValue)
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.To),
                    new[] { "ValidTo" });
            }

            if (Questions.IsNullOrEmpty())
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.Questions),
                    new[] { "Questions" });
            }
        }
    }
}