namespace Inspire.Model.QueryModels
{
    using System;

    using Inspire.Model.Nomenclature;

    public class FaqQueryModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public Nomenclature Category { get; set; }

        public Guid LanguageId { get; set; }

        public Guid? Status { get; set; }

        public string SearchWord { get; set; }
    }
}
