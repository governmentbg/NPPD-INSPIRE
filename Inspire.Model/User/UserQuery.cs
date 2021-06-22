namespace Inspire.Model.User
{
    using System;

    public class UserQuery
    {
        public Guid? Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Guid? Status { get; set; }

        public Guid? Organisation { get; set; }
    }
}