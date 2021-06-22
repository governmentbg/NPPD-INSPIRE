namespace Inspire.Portal.Models.GeoNetwork.Group
{
    using Inspire.Portal.Models.GeoNetwork.User;

    public class UserGroup
    {
        public UserGroupId Id { get; set; }

        public GroupDTO Group { get; set; }

        public GeoNetworkUser User { get; set; }

        public string Profile { get; set; }
    }
}