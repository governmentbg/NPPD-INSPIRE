namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    using System.Security.Principal;

    internal class OrNode : BinaryNode
    {
        public OrNode(Node leftExpression, Node rightExpression)
            : base(leftExpression, rightExpression)
        {
        }

        public override bool Eval(IPrincipal principal)
        {
            return LeftExpression.Eval(principal) || RightExpression.Eval(principal);
        }
    }
}