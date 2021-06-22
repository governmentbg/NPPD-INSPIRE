namespace Inspire.Core.Infrastructure.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public interface IUserPrincipal : IPrincipal
    {
        Guid Id { get; }

        string UserName { get; }

        string Email { get; }

        string Name { get; }

        string Password { get; }

        IEnumerable<string> Roles { get; }

        IEnumerable<string> RoleActivities { get; }

        Guid DbSessionId { get; }

        bool IsAdmin { get; }
    }
}