using System.Collections;
using System.Linq.Expressions;
using System.Text;

namespace QueryableCsv;

internal class CsvQueryProvider : IQueryProvider
{
    private readonly ObjectMapper _objectMapper;
    private readonly bool _disposeReader;
    private readonly Func<TextReader> _getReader;

    internal CsvQueryProvider(string path, ObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
        _disposeReader = true;
        _getReader = () => new StreamReader(path, Encoding.UTF8);
    }

    internal CsvQueryProvider(TextReader reader, ObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
        _disposeReader = false;
        _getReader = () => reader;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException();
    }

    public object? Execute(Expression expression)
    {
        throw new NotSupportedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new QueryableCsv<TElement>(this, expression);
    }

    /// Execute enumerable queries.
    internal IEnumerator<TResult> ExecuteEnumerator<TResult>(Expression expression)
    {
        var filterVisitor = new FilterVisitor();
        var filteredExpression = filterVisitor.Visit(expression);

        var objects = ReadFile(filterVisitor);

        // Use Linq to Objects to execute rest of the query
        var queryable = objects.AsQueryable();
        var queryableVisitor = new AsQueryableVisitor(queryable);
        var queryableExpression = queryableVisitor.Visit(filteredExpression);

        return queryable.Provider.CreateQuery<TResult>(queryableExpression).GetEnumerator();
    }
    
    // The Execute method executes queries that return a single value (instead of an enumerable sequence of values).
    // Expression trees that represent queries that return enumerable results are executed when
    // the IQueryable T object that contains the expression tree is enumerated.
    // 
    // The Queryable standard query operator methods that return singleton results call Execute.
    // They pass it a MethodCallExpression that represents a LINQ query.
    public TResult Execute<TResult>(Expression expression)
    {
        var filterVisitor = new FilterVisitor();
        var filtered = filterVisitor.Visit(expression);

        var objects = ReadFile(filterVisitor);

        // Use Linq to Objects to execute rest of the query
        var asQueryable = objects.AsQueryable();
        var queryableVisitor = new AsQueryableVisitor(asQueryable);
        var enumerableExpression = queryableVisitor.Visit(filtered);

        // Probably need to remove filters from tree so they are not executed twice?
        // Maybe this works???
        return asQueryable.Provider.Execute<TResult>(enumerableExpression);
    }

    private IEnumerable ReadFile(FilterVisitor filterVisitor)
    {
        var reader = _getReader();
        try
        {
            return _objectMapper.ReadFile(filterVisitor, reader);
        }
        finally
        {
            if (_disposeReader)
            {
                reader.Dispose();
            }
        }
    }

}