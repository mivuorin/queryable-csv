using System.Linq.Expressions;

namespace QueryableCsv;

internal class AsQueryableVisitor : ExpressionVisitor
{
    private readonly IQueryable _queryable;

    public AsQueryableVisitor(IQueryable queryable)
    {
        _queryable = queryable;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(QueryableCsv<>))
        {
            return Expression.Constant(_queryable);
        }

        return base.VisitConstant(node);
    }
}