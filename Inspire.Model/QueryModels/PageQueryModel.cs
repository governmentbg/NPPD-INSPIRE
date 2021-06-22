namespace Inspire.Model.QueryModels
{
    using System;

    public class PageQueryModel
    {
        public Guid? Id { get; set; }

        public string PermanentLink { get; set; }

        public Guid? TypeId { get; set; }

        public Guid? VisibilityTypeId { get; set; }
    }
}
