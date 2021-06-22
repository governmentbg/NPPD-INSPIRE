namespace Inspire.Portal.Areas.Admin.Models.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Portal.App_GlobalResources;

    public class ChangeStatus
    {
        public Guid UserId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public Guid StatusId { get; set; }
    }
}