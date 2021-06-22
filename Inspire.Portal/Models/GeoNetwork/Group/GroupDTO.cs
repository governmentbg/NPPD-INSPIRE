namespace Inspire.Portal.Models.GeoNetwork.Group
{
    using System.Collections.Generic;

    public class GroupDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Referrer { get; set; }

        public string Email { get; set; }

        public string Logo { get; set; }

        public string Website { get; set; }

        public Category DefaultCategory { get; set; }

        public bool EnableAllowedCategories { get; set; }

        public IEnumerable<Category> AllowedCategories { get; set; }

        public Dictionary<string, string> Label { get; set; }
    }
}