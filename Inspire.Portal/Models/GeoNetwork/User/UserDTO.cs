namespace Inspire.Portal.Models.GeoNetwork.User
{
    using Newtonsoft.Json;

    public class UserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Enabled { get; set; }

        public string Surname { get; set; }

        public string Username { get; set; }

        public string[] EmailAddresses { get; set; }

        public string[] GroupsRegisteredUser { get; set; }

        public string[] GroupsEditor { get; set; }

        public string[] GroupsReviewer { get; set; }

        public string[] GroupsUserAdmin { get; set; }

        public string Profile { get; set; }

        [JsonProperty("Organisation")]
        public string Organization { get; set; }

        public GeoNetworkAddress[] Addresses { get; set; }

        public string Kind { get; set; } = string.Empty;
    }
}