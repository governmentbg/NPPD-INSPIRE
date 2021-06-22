namespace Inspire.Model.Poll
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public class PollResult : BaseDbModel
    {
        public string Ip { get; set; }

        public List<string> QuestionTitles { get; set; }

        public List<string> QuestionAnswers { get; set; }

        public DateTime RegDate { get; set; }
    }
}
