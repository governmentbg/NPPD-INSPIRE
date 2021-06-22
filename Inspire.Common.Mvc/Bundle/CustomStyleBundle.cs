namespace Inspire.Common.Mvc.Bundle
{
    using System;
    using System.Web.Optimization;

    public sealed class CustomStyleBundle : Bundle
    {
        public CustomStyleBundle(string virtualPath)
            : this(virtualPath, null)
        {
        }

        public CustomStyleBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath, null)
        {
            ConcatenationToken = Environment.NewLine;
            Builder = new CustomStyleBundleBuilder();
        }
    }
}