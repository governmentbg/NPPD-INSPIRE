namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    internal abstract class BinaryNode : Node
    {
        protected BinaryNode(Node leftExpression, Node rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public Node LeftExpression { get; }

        public Node RightExpression { get; }
    }
}