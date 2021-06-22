namespace Inspire.Model.Poll
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Poll : BaseDbModel
    {
        public SortedDictionary<string, string> Titles { get; set; }

        public SortedDictionary<string, string> Descriptions { get; set; }

        public List<Question> Questions { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public string Author { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime LastUpdate { get; set; }

        public Nomenclature Status { get; set; }
    }
}
