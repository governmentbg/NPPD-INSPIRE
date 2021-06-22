namespace Inspire.Portal.Areas.Admin.Models.User
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.Group;
    using Inspire.Portal.Models.GeoNetwork.User;

    public class UserUpsertViewModel : BaseDbModel
    {
        [Required]
        [StringLength(
            500,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string UserName { get; set; }

        [StringLength(
            1000,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "FirstName")]
        public string Name { get; set; }

        [StringLength(
            1000,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Surname")]
        public string Surname { get; set; }

        [StringLength(
            500,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Phone")]
        public string Phone { get; set; }

        [StringLength(
            500,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        [Display(ResourceType = typeof(Resource), Name = "Fax")]
        public string Fax { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        [RegularExpression(
            "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$",
            ErrorMessageResourceName = "RegularExpressionValidationError",
            ErrorMessageResourceType = typeof(Resource))]
        [StringLength(
            1000,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "StringLengthAttribute_ValidationError")]
        public string Email { get; set; }

        public GroupForUser Group { get; set; }

        public Nomenclature Profile { get; set; }

        public bool IsAdministrator { get; set; }

        public GeoNetworkAddress GeoNetworkAddress { get; set; }

        public long? GeoNetworkId { get; set; }

        public List<Guid> Roles { get; set; }
    }
}