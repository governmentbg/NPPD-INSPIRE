namespace Inspire.Model.User
{
    using System;
    using System.Collections.Generic;

    public class SetRole
    {
        public Guid UserId { get; set; }

        public List<Guid> Roles { get; set; }
    }
}