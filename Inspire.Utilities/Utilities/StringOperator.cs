namespace Inspire.Utilities.Utilities
{
    using System;

    public static class StringOperator
    {
        /// <summary>
        ///     Examples:
        ///     searchString = "ab", then return string = "%ab%"
        ///     searchString = "a  b", then return string = "%a%%b%"
        ///     searchString = "*a*b", then return string = "%a%b%"
        ///     searchString = "a%b", then return string = "%a%b%"
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static string GetCorrectSearchString(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return null;
            }

            var correctSearchString = ReplaceSearchSymbol(searchString);
            correctSearchString = correctSearchString.Replace(" ", "%");

            if (!correctSearchString.StartsWith("%"))
            {
                correctSearchString = correctSearchString.Insert(0, "%");
            }

            if (!correctSearchString.EndsWith("%"))
            {
                correctSearchString = correctSearchString.Insert(correctSearchString.Length, "%");
            }

            return correctSearchString;
        }

        /// <summary>
        ///     Replace only first find string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos >= 0)
            {
                return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
            }

            return text;
        }

        public static string ReplaceSearchSymbol(string searchString)
        {
            return string.IsNullOrEmpty(searchString) ? null : searchString.Replace("*", "%").Trim();
        }
    }
}