namespace Inspire.Portal.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Extensions;

    public class ChangePasswordViewModel : IValidatableObject
    {
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resource), Name = "OldPassword")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resource), Name = "NewPassword")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(
            "Password",
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordsDoesNotMatch")]
        [Display(ResourceType = typeof(Resource), Name = "ConfirmNewPassword")]
        public string ConfirmPassword { get; set; }

        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }

        public string Token { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Token.IsNullOrEmpty() && OldPassword.IsNullOrEmpty())
            {
                yield return new ValidationResult(
                    string.Format(Resource.Required, Resource.OldPassword),
                    new[] { "OldPassword" });
            }

            var regex = new Regex(
                IsAdmin ? ConfigurationReader.AdminPasswordRegex : ConfigurationReader.UserPasswordRegex);
            if (!regex.IsMatch(Password))
            {
                yield return new ValidationResult(
                    IsAdmin ? Resource.AdminPasswordValidationMessage : Resource.PasswordValidationMessage,
                    new[] { "Password" });
            }
        }
    }
}