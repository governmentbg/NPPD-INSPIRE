namespace Inspire.Portal.Areas.Admin.Models.UserLogin
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;

    public class UserLoginQueryViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? LoginDateFrom { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? LoginDateTo { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ip")]
        public string LoginIp { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "InGeoPortal")]
        public bool IsFromGN { get; set; }
    }
}
