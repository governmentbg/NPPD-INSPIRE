namespace Inspire.Model.TableModels
{
    using System;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class PollTableModel : BaseDbModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Nomenclature Status { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime? RegDate { get; set; }

        public string Author { get; set; }
    }
}
