namespace Inspire.Model.Group
{
    using Inspire.Model.Base;

    public class GroupTableModel : BaseDbModel
    {
        public string Bulstat { get; set; }

        public string Email { get; set; }

        public long? GeoNetworkId { get; set; }

        public string Name { get; set; }

        public string ContactPerson { get; set; }

        public string Logo { get; set; }
    }
}
