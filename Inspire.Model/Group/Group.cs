namespace Inspire.Model.Group
{
    using System.Collections.Generic;
    using Inspire.Model.Base;

    public class Group : BaseDbModel
    {
        public string Bulstat { get; set; }

        public string Email { get; set; }

        public long? GeoNetworkId { get; set; }

        public SortedDictionary<string, string> Names { get; set; }

        public SortedDictionary<string, string> ContactPersons { get; set; }

        public SortedDictionary<string, string> Description { get; set; }

        public SortedDictionary<string, string> Website { get; set; }
    }
}
