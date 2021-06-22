namespace Inspire.Repository.Utilities
{
    using System;
    using System.Collections.Generic;

    using Inspire.Utilities.Extensions;

    public class CultureHelper
    {
        public static SortedDictionary<string, string> GetDictionaryData(Guid[] languageIds, string[] data)
        {
            if (data.IsNullOrEmpty() || languageIds.IsNullOrEmpty() || data.Length != languageIds.Length)
            {
                return null;
            }

            var result = new SortedDictionary<string, string>();
            for (var i = 0; i < languageIds.Length; i++)
            {
                result.Add(languageIds[i].ToString(), data[i]);
            }

            return result;
        }
    }
}