namespace Inspire.Portal.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;

    public class LoginViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resource), Name = "Password")]
        public string Password { get; set; }
    }
}