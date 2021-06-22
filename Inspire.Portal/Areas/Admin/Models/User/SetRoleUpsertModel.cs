namespace Inspire.Portal.Areas.Admin.Models.User
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Extensions;

    public class SetRoleUpsertModel : IValidatableObject
    {
        public Guid UserId { get; set; }

        public List<Guid> Roles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Roles.IsNullOrEmpty())
            {
                yield return new ValidationResult(string.Format(Resource.Required, Resource.Roles), new[] { "Roles" });
            }
        }
    }
}