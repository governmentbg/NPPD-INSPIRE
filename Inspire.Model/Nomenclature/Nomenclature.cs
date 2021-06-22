namespace Inspire.Model.Nomenclature
{
    using Inspire.Model.Base;

    public class Nomenclature : BaseDbModel
    {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public string FullName => $"{Code} {Name}".Trim();
    }
}