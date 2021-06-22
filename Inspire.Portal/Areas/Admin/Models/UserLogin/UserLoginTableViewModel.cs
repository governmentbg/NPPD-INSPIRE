namespace Inspire.Portal.Areas.Admin.Models.UserLogin
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;

    public class UserLoginTableViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Date")]
        [TableOptions(Format = "g")]
        public DateTime? LoginDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ip")]
        public string LoginIp { get; set; }
    }
}
