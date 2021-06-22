namespace Inspire.Portal.Models.Publication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Model.Attachment;
    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class PublicationPublicViewModel : BaseDbModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? EnterDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<Attachment> Pictures { get; set; }

        public Nomenclature Type { get; set; }

        public bool IsLead { get; set; }

        public bool IsVisibleInWeb { get; set; }

        public Attachment MainPicture => Pictures?.SingleOrDefault(item => item.IsMain);

        public string Date =>
            EndDate.HasValue
                ? $"{StartDate:d} - {EndDate:d}"
                : $"{StartDate:d}";
    }
}