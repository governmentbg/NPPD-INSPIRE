namespace Inspire.Model.Role
{
    using System.Collections.Generic;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class Role : BaseDbModel
    {
        public string Name { get; set; }

        public List<Nomenclature> Activities { get; set; }
    }
}