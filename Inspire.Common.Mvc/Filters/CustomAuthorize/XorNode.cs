namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    using System.Security.Principal;

    internal class XorNode : BinaryNode
    {
        public XorNode(Node leftExpression, Node rightExpression)
            : base(leftExpression, rightExpression)
        {
        }

        public override bool Eval(IPrincipal principal)
        {
            return LeftExpression.Eval(principal) ^ RightExpression.Eval(principal);
        }
    }
}