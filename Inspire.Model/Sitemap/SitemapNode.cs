namespace Inspire.Model.Sitemap
{
    using System;

    public class SitemapNode
    {
        public string Url { get; set; }

        public SitemapFrequency? Frequency { get; set; }

        public DateTime? LastModified { get; set; }

        public double? Priority { get; set; }
    }
}
