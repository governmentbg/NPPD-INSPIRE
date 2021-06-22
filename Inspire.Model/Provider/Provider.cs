namespace Inspire.Model.Provider
{
    using System;
    using System.Collections.Generic;
    using Inspire.Model.Attachment;
    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Provider : BaseDbModel
    {
        public SortedDictionary<string, string> Links { get; set; }

        public Attachment MainPicture { get; set; }

        public SortedDictionary<string, string> Names { get; set; }

        public SortedDictionary<string, string> Descriptions { get; set; }

        public Nomenclature Status { get; set; }

        public Guid? LanguageId { get; set; }
    }
}
