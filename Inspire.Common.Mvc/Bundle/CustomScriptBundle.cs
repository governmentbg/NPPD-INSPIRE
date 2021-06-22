namespace Inspire.Common.Mvc.Bundle
{
    using System;
    using System.Web.Optimization;

    public sealed class CustomScriptBundle : Bundle
    {
        public CustomScriptBundle(string virtualPath)
            : this(virtualPath, null)
        {
        }

        public CustomScriptBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath, null)
        {
            ConcatenationToken = ";" + Environment.NewLine;
            Builder = new CustomJsBundleBuilder();
        }
    }
}