using System.Collections;
using System.Linq.Expressions;

namespace QueryableCsv;

// IQueryable just holds expression tree and query provider.
// QueryProvider creates multiple queryable instances with new expression tree every time some Query operator is called. eg. Where.
// TODO Write documentation and usage example.
public class QueryableCsv<T> : IQueryable<T>
{
    private readonly CsvQueryProvider _provider;

    // TODO Maybe move to different factory etc. eg. CsvQuery.File<T>("asdasd")
    public QueryableCsv(string path)
    {
        var columnType = typeof(T);
        _provider = new CsvQueryProvider(path, new ObjectMapper(columnType));

        // Linq requires that expression tree's root node to be IQueryable<'T> implementation.
        Expression = Expression.Constant(this);
    }

    public QueryableCsv(TextReader reader)
    {
        var columnType = typeof(T);
        _provider = new CsvQueryProvider(reader, new ObjectMapper(columnType));

        // Linq requires that expression tree's root node to be IQueryable<'T> implementation.
        Expression = Expression.Constant(this);
    }

    internal QueryableCsv(CsvQueryProvider provider, Expression expression)
    {
        _provider = provider;
        Expression = expression;
    }

    public IEnumerator<T> GetEnumerator()
    {
        // Enumeration forces the expression tree associated with an IQueryable<T> object to be executed
        // The definition of "executing an expression tree" is specific to a query provider
        return _provider.ExecuteEnumerator<T>(Expression);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType => typeof(T);
    public Expression Expression { get; }
    public IQueryProvider Provider => _provider;
}