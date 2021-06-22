namespace Inspire.Portal.Models.Provider
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Inspire.Model.Attachment;
    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Extensions;

    public class ProviderViewModel : BaseDbModel, IValidatableObject
    {
        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Link")]
        public SortedDictionary<string, string> Links { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Image")]
        public Attachment MainPicture { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public SortedDictionary<string, string> Names { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Text")]
        public SortedDictionary<string, string> Descriptions { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public Nomenclature Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MainPicture?.Url))
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.Image),
                    new[] { "MainPictureUrl" });
            }
        }
    }
}