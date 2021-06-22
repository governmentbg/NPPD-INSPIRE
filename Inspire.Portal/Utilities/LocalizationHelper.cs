namespace Inspire.Portal.Utilities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;

    using Inspire.Utilities.Extensions;

    public static class LocalizationHelper
    {
        public static string GetCurrentCultureId()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            return Global.Cultures[currentCulture];
        }

        public static string GetValueForCurrentCulture(this IDictionary<string, string> languageData)
        {
            var cultureId = GetCurrentCultureId();
            return languageData?.ContainsKey(cultureId) == true
                ? languageData[cultureId]
                : string.Empty;
        }

        internal static IEnumerable<ValidationResult> Validate(
            this SortedDictionary<string, string> languageData,
            string errorMessage,
            string memberName)
        {
            if (languageData.IsNullOrEmpty())
            {
                yield return new ValidationResult(errorMessage, new[] { memberName });
            }
            else
            {
                foreach (var key in Global.Languages.Where(i => i.Required).Select(i => i.Id.ToString()))
                {
                    if (!languageData.ContainsKey(key) || languageData[key].IsNullOrEmpty())
                    {
                        yield return new ValidationResult(errorMessage, new[] { $"{memberName}[{key}]" });
                    }
                }
            }
        }
    }
}