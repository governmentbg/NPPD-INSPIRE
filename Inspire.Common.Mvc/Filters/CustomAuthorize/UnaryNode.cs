namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    internal abstract class UnaryNode : Node
    {
        protected UnaryNode(Node expression)
        {
            Expression = expression;
        }

        public Node Expression { get; }
    }
}