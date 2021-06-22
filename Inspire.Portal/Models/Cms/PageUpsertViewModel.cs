namespace Inspire.Portal.Models.Cms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    using Inspire.Common.Mvc.Filters;
    using Inspire.Model.Base;
    using Inspire.Model.Cms;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class PageUpsertViewModel : BaseDbModel, IValidatableObject
    {
        private string permanentLink;

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Type")]
        public Nomenclature Type { get; set; }

        [Required]
        [RegularExpressionWithOptions(@"[(http(s)?):\/\/(www\.)?a-zA-Z0-9@:%._\+~#=]{2,256}(\.[a-z]{2,6})?\b([-a-zA-Z0-9@:%_\+.~#?&\/\/=]*({{lngiso2}})*({{lngiso3}})*)*|(\/)?[a-zA-Z][-a-zA-Z0-9@:%_\+.~#?&//=]|#*", RegexOptions = RegexOptions.IgnoreCase, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidLink")]
        [Display(ResourceType = typeof(Resource), Name = "Link")]
        public string PermanentLink
        {
            get => permanentLink;

            set => permanentLink = value?.Trim().TrimStart('/');
        }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Public")]
        public Nomenclature VisibilityType { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Location")]
        public Nomenclature LocationType { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "OpenInNewWindow")]
        public bool IsInNewWindow { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ParentPage")]
        public Guid? ParentId { get; set; }

        public string Title => Titles.GetValueForCurrentCulture();

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public SortedDictionary<string, string> Titles { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MenuTitle")]
        public SortedDictionary<string, string> TitlesMenu { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Content")]
        public SortedDictionary<string, string> Contents { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Keywords")]
        public SortedDictionary<string, string> Keywords { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type?.Id.HasValue != true)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Type), new[] { "Type" });
            }
            else
            {
                switch (EnumHelper.GetPageTypeById(Type.Id.Value))
                {
                    case PageType.Content:
                        {
                            foreach (var validationResult in Contents.Validate(string.Format(Resource.Required, Resource.Content), "Contents"))
                            {
                                yield return validationResult;
                            }

                            break;
                        }
                }
            }

            if (VisibilityType?.Id.HasValue != true)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Public), new[] { "VisibilityType" });
            }

            if (LocationType?.Id.HasValue != true)
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Location), new[] { "LocationType" });
            }

            if (Id.HasValue && Id == ParentId)
            {
                yield return new ValidationResult(string.Format(Resource.InvalidFieldValue, Resource.ParentPage), new[] { "ParentId" });
            }

            foreach (var validationResult in Titles.Validate(string.Format(Resource.Required, Resource.Title), "Titles"))
            {
                yield return validationResult;
            }
        }
    }
}