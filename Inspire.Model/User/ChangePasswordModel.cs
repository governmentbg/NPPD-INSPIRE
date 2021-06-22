namespace Inspire.Model.User
{
    using System;

    public class ChangePasswordModel
    {
        public Guid UserId { get; set; }

        public string Password { get; set; }
    }
}