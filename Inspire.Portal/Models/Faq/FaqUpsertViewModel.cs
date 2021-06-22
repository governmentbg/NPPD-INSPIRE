namespace Inspire.Portal.Models.Faq
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class FaqUpsertViewModel : BaseDbModel, IValidatableObject
    {
        [Display(ResourceType = typeof(Resource), Name = "Question")]
        public string Question => Questions.GetValueForCurrentCulture();

        [Display(ResourceType = typeof(Resource), Name = "Answer")]
        public string Answer => Answers.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Question")]
        public SortedDictionary<string, string> Questions { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Answer")]
        public SortedDictionary<string, string> Answers { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Category")]
        public Nomenclature Category { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public Nomenclature Status { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ArchiveDate")]
        public DateTime? ScheduledArchiveDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Category?.Id == null)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Category), new[] { "Category.Id" });
            }

            if (Status?.Id == null)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Category), new[] { "Status" });
            }

            foreach (var validationResult in Questions.Validate(string.Format(Resource.Required, Resource.Question), "Questions"))
            {
                yield return validationResult;
            }

            foreach (var validationResult in Answers.Validate(string.Format(Resource.Required, Resource.Answer), "Answers"))
            {
                yield return validationResult;
            }
        }
    }
}