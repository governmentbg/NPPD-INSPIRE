namespace Inspire.Portal.Models
{
    using System.Collections.Generic;

    public class MenuItem
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Class { get; set; }

        public IEnumerable<MenuItem> Items { get; set; }

        public bool IsAjax { get; set; }

        public string HttpMethod { get; set; }

        public bool InNewWindow { get; set; }
    }
}