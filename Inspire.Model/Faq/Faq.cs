namespace Inspire.Model.Faq
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Faq : BaseDbModel
    {
        public SortedDictionary<string, string> Questions { get; set; }

        public SortedDictionary<string, string> Answers { get; set; }

        public Nomenclature Category { get; set; }

        public Nomenclature Status { get; set; }

        public DateTime? RegDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? ScheduledArchiveDate { get; set; }

        public DateTime? ActualArchiveDate { get; set; }
    }
}
