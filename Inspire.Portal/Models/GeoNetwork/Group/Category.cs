namespace Inspire.Portal.Models.GeoNetwork.Group
{
    using System.Collections.Generic;

    public class Category
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Label { get; set; }
    }
}