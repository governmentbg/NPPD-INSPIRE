namespace Inspire.Portal.Areas.Admin.Models.Role
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Extensions;

    public class RoleUpsertViewModel : BaseDbModel, IValidatableObject
    {
        [Required]
        [StringLength(
            1000,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Rights")]
        public List<Activity> Activities { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Activities.IsNullOrEmpty())
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Rights));
            }
        }
    }
}