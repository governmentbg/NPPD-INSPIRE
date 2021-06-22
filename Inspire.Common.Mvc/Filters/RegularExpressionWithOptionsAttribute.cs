namespace Inspire.Common.Mvc.Filters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RegularExpressionWithOptionsAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public RegularExpressionWithOptionsAttribute(string pattern)
            : base(pattern)
        {
        }

        public RegexOptions RegexOptions { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                       {
                           ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                           ValidationType = "regexwithoptions"
                       };

            rule.ValidationParameters["pattern"] = Pattern;

            var flags = string.Empty;
            if ((RegexOptions & RegexOptions.Multiline) == RegexOptions.Multiline)
            {
                flags += "m";
            }

            if ((RegexOptions & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
            {
                flags += "i";
            }

            rule.ValidationParameters["flags"] = flags;

            yield return rule;
        }

        public override bool IsValid(object value)
        {
            return string.IsNullOrEmpty(value as string) || Regex.IsMatch((string)value, $"^{Pattern}$", RegexOptions);
        }
    }
}