using System.Linq.Expressions;

namespace QueryableCsv;

/// <summary>
/// Visits expression tree and finds expressions which can be used to filtering when parsing csv lines.
/// Removes filtering expressions so that they are not executed twice.
/// </summary>
internal class FilterVisitor : ExpressionVisitor
{
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Queryable))
        {
            // TODO Support indexed versions
            if (node.Method.Name == nameof(Queryable.Where))
            {
                var source = node.Arguments[0];

                var unary = (UnaryExpression)node.Arguments[1];
                var lambda = (LambdaExpression)unary.Operand;

                Filter = lambda.Compile();

                // Remove method call from expression tree eg. object.Where( ... ) -> objects
                return source;
            }
        }

        return base.VisitMethodCall(node);
    }
    
    public Delegate? Filter { get; set; }
}