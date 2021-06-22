namespace Inspire.Portal.Models.Publication
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Attachment;
    using Inspire.Model.Base;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Publication;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class PublicationUpsertViewModel : BaseDbModel, IValidatableObject
    {
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Title => Titles.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public SortedDictionary<string, string> Titles { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Content")]
        public string Content => Contents.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Content")]
        public SortedDictionary<string, string> Contents { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EnterDate")]
        [DataType(DataType.Date)]
        public DateTime? EnterDate { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "StartDate")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EndDate")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public List<Attachment> Pictures { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Type")]
        public Nomenclature Type { get; set; }

        public bool IsLead { get; set; }

        public bool IsVisibleInWeb { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type?.Id.HasValue != true)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Type), new[] { "Type" });
            }
            else if (EnumHelper.GetPublicationTypeById(Type.Id.Value) == PublicationType.Event)
            {
                if (!EndDate.HasValue)
                {
                    yield return new ValidationResult(string.Format(Resource.Required, Resource.EndDate), new[] { "EndDate" });
                }
                else if (StartDate.HasValue && StartDate > EndDate)
                {
                    yield return new ValidationResult(string.Format(Resource.InvalidFieldValue, Resource.EndDate), new[] { "EndDate" });
                }
            }
        }
    }
}