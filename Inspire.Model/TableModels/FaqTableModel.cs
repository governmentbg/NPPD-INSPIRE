namespace Inspire.Model.TableModels
{
    using System;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class FaqTableModel : BaseDbModel
    {
        public string Question { get; set; }

        public string Answer { get; set; }

        public DateTime? RegDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string CategoryName => Category.Name;

        public Nomenclature Category { get; set; }

        public bool IsValid { get; set; }

        public Nomenclature Status { get; set; }
    }
}
