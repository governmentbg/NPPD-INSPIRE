namespace Inspire.Common.Mvc.Bundle
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Optimization;

    public class CustomJsBundleBuilder : IBundleBuilder
    {
        public string BuildBundleContent(Bundle bundle, BundleContext context, IEnumerable<BundleFile> files)
        {
            if (files == null)
            {
                return string.Empty;
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (bundle == null)
            {
                throw new ArgumentNullException("bundle");
            }

            var stringBuilder = new StringBuilder();
            foreach (var bundleFile in files)
            {
                bundleFile.Transforms.Add(
                    new SkipFileTransform(s => s.EndsWith(".min.js", StringComparison.InvariantCultureIgnoreCase)));
                stringBuilder.Append(bundleFile.ApplyTransforms());
                stringBuilder.Append(bundle.ConcatenationToken);
            }

            return stringBuilder.ToString();
        }
    }
}