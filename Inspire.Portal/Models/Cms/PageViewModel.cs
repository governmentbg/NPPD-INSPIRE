namespace Inspire.Portal.Models.Cms
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Inspire.Model.Base;
    using Inspire.Model.Cms;
    using Inspire.Portal.App_GlobalResources;

    public class PageViewModel : BaseDbModel
    {
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Content")]
        public string Content { get; set; }

        public string Keywords { get; set; }

        public string PermanentLink { get; set; }

        public Guid? ParentId { get; set; }

        public long Order { get; set; }

        public long DbId { get; set; }

        public long? ParentDbId { get; set; }

        public PageType PageType { get; set; }

        public VisibilityType Visibility { get; set; }
    }
}