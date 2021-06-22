namespace Inspire.Model.Poll
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Question : BaseDbModel
    {
        public SortedDictionary<string, string> Titles { get; set; }

        public SortedDictionary<string, string> Descriptions { get; set; }

        public Nomenclature Type { get; set; }

        public List<Option> Options { get; set; }

        public bool Mandatory { get; set; }

        public Guid? PollId { get; set; }
    }
}
