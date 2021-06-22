namespace Inspire.Model.Faq
{
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public class FaqCategory : BaseDbModel
    {
        public SortedDictionary<string, string> Names { get; set; }

        public bool IsValid { get; set; }

        public bool HasQuestions { get; set; }
    }
}
