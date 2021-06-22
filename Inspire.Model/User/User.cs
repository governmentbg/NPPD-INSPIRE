namespace Inspire.Model.User
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;
    using Inspire.Model.Nomenclature;

    public class User : BaseDbModel, IUser
    {
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Fax { get; set; }

        public virtual Nomenclature Status { get; set; }

        public virtual string Department { get; set; }

        public bool IsAdmin { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual string Token { get; set; }

        public virtual List<string> Roles { get; set; }

        public virtual List<Guid> RoleActivities { get; set; }

        public virtual long? GeoNetworkId { get; set; }

        public virtual string Organisation { get; set; }

        public virtual Guid? OrganizationId { get; set; }
    }
}