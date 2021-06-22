namespace Inspire.Model.Poll
{
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public class Option : BaseDbModel
    {
        public SortedDictionary<string, string> Values { get; set; }

        public bool IsSelected { get; set; }
    }
}
