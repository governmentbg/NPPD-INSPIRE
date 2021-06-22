namespace Inspire.Common.Mvc.Bundle
{
    using System;
    using System.Web.Optimization;

    using Microsoft.Ajax.Utilities;

    public class SkipFileTransform : IItemTransform
    {
        private readonly Predicate<string> skiPredicate;

        public SkipFileTransform(Predicate<string> skiPredicate)
        {
            this.skiPredicate = skiPredicate;
        }

        public string Process(string includedVirtualPath, string input)
        {
            if (skiPredicate(includedVirtualPath))
            {
                return input;
            }

            var minifier = new Minifier();
            var str = includedVirtualPath.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase)
                ? minifier.MinifyJavaScript(
                    input,
                    new CodeSettings
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false
                    })
                : minifier.MinifyStyleSheet(input, new CssSettings());
            if (minifier.ErrorList.Count > 0)
            {
                return "/* " + string.Concat(minifier.Errors) + " */";
            }

            return str;
        }
    }
}