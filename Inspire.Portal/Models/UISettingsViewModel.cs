namespace Inspire.Portal.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    public class UISettingsViewModel : BaseDbModel
    {
        [Display(Name = "Address", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> Address { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(Resource))]
        public string Phone { get; set; }

        [Display(Name = "WorkingTime", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> WorkingTime { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Display(Name = "Explanation", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> Explanation { get; set; }

        [Display(Name = "FaceBookLink", ResourceType = typeof(Resource))]
        public string FbLink { get; set; }

        [Display(Name = "LinkedInLink", ResourceType = typeof(Resource))]
        public string LinkedInLink { get; set; }

        [Display(Name = "PortalName", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> PortalName { get; set; }

        public string PortalNameByCulture => PortalName.GetValueForCurrentCulture();

        [Display(Name = "PortalSubName", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> PortalSubName { get; set; }

        public string PortalSubNameByCulture => PortalSubName.GetValueForCurrentCulture();

        [Display(Name = "PortalRights", ResourceType = typeof(Resource))]
        public SortedDictionary<string, string> PortalRights { get; set; }

        public string PortalRightsByCulture => PortalRights.GetValueForCurrentCulture();

        public string AddressByCulture => Address.GetValueForCurrentCulture();

        public string WorkingTimeByCulture => WorkingTime.GetValueForCurrentCulture();

        public string ExplanationByCulture => Explanation.GetValueForCurrentCulture();
    }
}