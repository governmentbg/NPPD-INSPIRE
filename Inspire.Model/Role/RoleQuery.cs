namespace Inspire.Model.Role
{
    using System;

    public class RoleQuery
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }
    }
}