namespace Inspire.Portal.Areas.Admin.Models.Group
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Infrastructure.Attribute;
    using Inspire.Portal.Models.GeoNetwork.Group;

    public class GroupUpsertModel : BaseDbModel
    {
        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Bulstat")]
        public string Bulstat { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(
            "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$",
            ErrorMessageResourceName = "RegularExpressionValidationError",
            ErrorMessageResourceType = typeof(Resource))]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        [DictionaryStringLength(32)]
        public SortedDictionary<string, string> Names { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ContactPerson")]
        public SortedDictionary<string, string> ContactPersons { get; set; }

        public long? GeoNetworkId { get; set; }

        [DictionaryStringLength(255)]
        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public SortedDictionary<string, string> Description { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Website")]
        public SortedDictionary<string, string> Website { get; set; }

        public string Logo { get; set; }

        public Category DefaultCategory { get; set; }

        public bool EnableAllowedCategories { get; set; }

        [DictionaryStringLength(255)]
        public Dictionary<string, string> Label { get; set; }

        public List<long> SelectedAllowedCategories { get; set; }
    }
}