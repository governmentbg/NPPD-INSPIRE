namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    using System.Security.Principal;

    internal class RoleNode : Node
    {
        public RoleNode(string roleName)
        {
            RoleName = roleName;
        }

        public string RoleName { get; }

        public override bool Eval(IPrincipal principal)
        {
            return principal.IsInRole(RoleName);
        }
    }
}