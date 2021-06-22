namespace Inspire.Portal.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;

    public class ForgottenPasswordViewModel
    {
        [Required]
        [Display(Name = "Username", ResourceType = typeof(Resource))]
        public string UserName { get; set; }
    }
}