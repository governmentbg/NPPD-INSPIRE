namespace Inspire.Model.Admin
{
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public class UISettingsModel : BaseDbModel
    {
        public SortedDictionary<string, string> Address { get; set; }

        public string Phone { get; set; }

        public SortedDictionary<string, string> WorkingTime { get; set; }

        public string Email { get; set; }

        public SortedDictionary<string, string> Explanation { get; set; }

        public string FbLink { get; set; }

        public string LinkedInLink { get; set; }

        public SortedDictionary<string, string> PortalName { get; set; }

        public SortedDictionary<string, string> PortalSubName { get; set; }

        public SortedDictionary<string, string> PortalRights { get; set; }
    }
}