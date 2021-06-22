namespace Inspire.Model.User
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Base;

    public interface IUser : IBaseDbModel
    {
        string UserName { get; set; }

        string Email { get; set; }

        string Password { get; set; }

        string Token { get; set; }

        List<string> Roles { get; set; }

        List<Guid> RoleActivities { get; set; }
    }
}