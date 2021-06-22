namespace Inspire.Portal.Models.Poll
{
    using System.Collections.Generic;

    public class PollResultViewModel
    {
        public string Ip { get; set; }

        public List<string> QuestionTitles { get; set; }

        public List<string> QuestionAnswers { get; set; }

        public string RegDate { get; set; }
    }
}