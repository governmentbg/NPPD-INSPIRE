namespace Inspire.Model.User
{
    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class UserControlItem : BaseDbModel
    {
        public Nomenclature Role { get; set; }

        public Nomenclature User { get; set; }
    }
}