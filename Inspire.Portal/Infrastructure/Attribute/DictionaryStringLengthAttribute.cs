namespace Inspire.Portal.Infrastructure.Attribute
{
    using System;
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Extensions;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DictionaryStringLengthAttribute : StringLengthAttribute
    {
        public DictionaryStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {
            ErrorMessageResourceType = typeof(Resource);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessageResourceName = ErrorMessageResourceName.IsNotNullOrEmpty()
                ? ErrorMessageResourceName
                : MinimumLength > 0
                    ? "StringLengthAttribute_ValidationErrorIncludingMinimum"
                    : "StringLengthAttribute_ValidationError";

            var dictionary = value as IDictionary;
            if (dictionary == null)
            {
                return ValidationResult.Success;
            }

            var invalidMembers = (from object key in dictionary.Keys
                                  let item = dictionary[key]?.ToString()
                                  where item?.Length > MaximumLength || item?.Length < MinimumLength
                                  select $"{validationContext.MemberName}[{key}]").ToList();

            return invalidMembers.IsNotNullOrEmpty()
                ? new ValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    new[] { validationContext.MemberName })
                : ValidationResult.Success;
        }
    }
}