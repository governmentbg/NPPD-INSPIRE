namespace Inspire.Model.Publication
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Attachment;
    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Publication : BaseDbModel
    {
        public SortedDictionary<string, string> Titles { get; set; }

        public SortedDictionary<string, string> Contents { get; set; }

        public DateTime? EnterDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<Attachment> Pictures { get; set; }

        public Nomenclature Type { get; set; }

        public bool IsLead { get; set; }

        public bool IsVisibleInWeb { get; set; }
    }
}