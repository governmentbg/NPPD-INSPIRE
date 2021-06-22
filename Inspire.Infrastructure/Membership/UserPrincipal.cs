namespace Inspire.Infrastructure.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;

    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Utilities.Extensions;

    public class UserPrincipal : IUserPrincipal
    {
        private IIdentity identity;
        private IEnumerable<string> roleActivities;

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public Guid DbSessionId { get; set; }

        public bool IsAdmin { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IIdentity Identity => identity ?? (identity = UserName.IsNotNullOrEmpty() ? new GenericIdentity(UserName) : null);

        public IEnumerable<string> RoleActivities
        {
            get => roleActivities ?? (roleActivities = new List<string>());

            set => roleActivities = value;
        }

        public bool IsInRole(string role)
        {
            return RoleActivities.IsNotNullOrEmpty() && RoleActivities.Contains(role);
        }
    }
}