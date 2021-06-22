namespace Inspire.Model.Publication
{
    using System;

    public class PublicationQuery
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public Guid? TypeId { get; set; }

        public Guid LanguageId { get; set; }

        public DateTime? EnterDateFrom { get; set; }

        public DateTime? EnterDateTo { get; set; }

        public DateTime? StartDateFrom { get; set; }

        public DateTime? StartDateTo { get; set; }

        public bool? IsVisible { get; set; }
    }
}