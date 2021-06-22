namespace Inspire.Common.Mvc.Bundle
{
    using System.Collections.Generic;
    using System.Web.Optimization;

    public class SyncBundleOrder : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}