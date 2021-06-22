namespace Inspire.Common.Mvc.Bundle
{
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Optimization;

    public class CustomCssRewriteUrlTransform : IItemTransform
    {
        private static readonly Regex Pattern = new Regex(
            @"url\s*\(\s*([""']?)([^:)]+)\1\s*\)",
            RegexOptions.IgnoreCase);

        public string Process(string includedVirtualPath, string input)
        {
            var matches = Pattern.Matches(input);
            if (matches.Count > 0)
            {
                var directoryPath = VirtualPathUtility.GetDirectory(includedVirtualPath);
                foreach (Match match in matches)
                {
                    var fileRelativePath = match.Groups[2].Value;
                    var fileVirtualPath = VirtualPathUtility.Combine(directoryPath, fileRelativePath);
                    var quote = match.Groups[1].Value;
                    var replace = string.Format(
                        "url({0}{1}{0})",
                        quote,
                        VirtualPathUtility.ToAbsolute(fileVirtualPath));

                    ////replace = new CssRewriteUrlTransform().Process(directoryPath, replace);
                    input = input.Replace(match.Groups[0].Value, replace);
                }
            }

            return input; // new CssRewriteUrlTransform().Process(includedVirtualPath, input);
        }
    }
}