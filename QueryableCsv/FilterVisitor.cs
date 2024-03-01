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
            if (node.Method.Name == nameof(Queryable.Where))
            {
                var source = node.Arguments[0];

                var unary = (UnaryExpression)node.Arguments[1];
                var filter = (LambdaExpression)unary.Operand;
                
                // Convert filter function to indexed version if needed.
                if (filter.Parameters.Count == 1)
                {
                    var obj = filter.Parameters[0];
                    var index = Expression.Parameter(typeof(int), "i");
                    
                    var invoke = Expression.Invoke(filter, obj);
                    var wrapped = Expression.Lambda(invoke, obj, index);
                    
                    FilterWithIndex = wrapped.Compile();
                }
                else
                {
                    
                    FilterWithIndex = filter.Compile();
                }

                // Remove method call from expression tree eg. object.Where( ... ) -> objects
                return source;
            }
        }

        return base.VisitMethodCall(node);
    }

    // TODO Possible to strongly type filter delegate?
    public Delegate? FilterWithIndex { get; set; }
}