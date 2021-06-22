namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    using System.Security.Principal;

    internal class NotNode : UnaryNode
    {
        public NotNode(Node expression)
            : base(expression)
        {
        }

        public override bool Eval(IPrincipal principal)
        {
            return !Expression.Eval(principal);
        }
    }
}