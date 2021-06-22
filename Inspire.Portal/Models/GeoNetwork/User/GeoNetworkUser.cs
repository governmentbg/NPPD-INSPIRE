namespace Inspire.Portal.Models.GeoNetwork.User
{
    using System;

    public class GeoNetworkUser
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Username { get; set; }

        public string Organization { get; set; }

        public bool AccountNonExpired { get; set; }

        public bool AccountNonLocked { get; set; }

        public GeoNetworkAddress PrimaryAddress { get; set; }

        public GeoNetworkAddress[] Addresses { get; set; }

        public string[] Authorities { get; set; }

        public bool CredentialsNonExpired { get; set; }

        public string Email { get; set; }

        public string[] EmailAddresses { get; set; }

        public bool Enabled { get; set; }

        public string Kind { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public string Profile { get; set; }
    }
}