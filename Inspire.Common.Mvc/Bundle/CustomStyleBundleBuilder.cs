namespace Inspire.Common.Mvc.Bundle
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Optimization;

    public class CustomStyleBundleBuilder : IBundleBuilder
    {
        public string BuildBundleContent(Bundle bundle, BundleContext context, IEnumerable<BundleFile> files)
        {
            var stringBuilder = new StringBuilder();
            foreach (var bundleFile in files)
            {
                bundleFile.Transforms.Add(
                    new SkipFileTransform(s => s.EndsWith(".min.css", StringComparison.InvariantCultureIgnoreCase)));
                bundleFile.Transforms.Add(new CustomCssRewriteUrlTransform());

                stringBuilder.Append(bundleFile.ApplyTransforms());
                stringBuilder.Append(bundle.ConcatenationToken);
            }

            return stringBuilder.ToString();
        }
    }
}