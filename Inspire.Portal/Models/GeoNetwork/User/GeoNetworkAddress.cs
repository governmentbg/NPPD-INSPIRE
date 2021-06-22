namespace Inspire.Portal.Models.GeoNetwork.User
{
    public class GeoNetworkAddress
    {
        public long? Id { get; set; }

        public string Address { get; set; }

        public string State { get; set; }

        public string Country { get; set; } = "Bulgaria";

        public string Zip { get; set; }

        public string City { get; set; }
    }
}