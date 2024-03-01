# Queryable Csv - Linq queries against csv files.

Revisited my old expression tree study which implements Linq to CSV files.

## TODO

* Improve mapping to support header row and name based mapping instead of indexes
* Configurable column separator
* Escape character, probably needs to be configurable

## How to use?

TODO: Write some nice example use cases based on unit tests.

## How it works?

TODO: Describe features shortly and how it works.

### Filtering

QueryableCsv parses given query for filtering operations eg. Where and uses it to filter objects before it passes rest
of the query to be executed as standard Linq to Objects query.

### Object mapping

Requires DTO's to implement parameterless constructor and public properties which are used in mapping. All mapped
properties need to be decorated with CsvColumn attribute.

### Map and read only selected columns optimization

Original idea was to use Linq Select operation to determine which columns needed to be read and mapped instead of
mapping all of them for every line read as optimization.

Csv lines are mapped into objects which are dynamically instantiated with parameterless constructor which does not work
for anonymous classes, so it would require to use dynamic types or some weak typed data structure, which would increase
complexity of object mapping logic.

Anonymous types can be created with `Activator.CreateInstance` overload which takes constructor arguments, but this
would require assumption that constructor arguments are in specific order or some ordering mechanism for mapped
properties.

## References

[MSDN Walkthrough: Creating an IQueryable LINQ Provider](https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2008/bb546158(v=vs.90))

[MSDN How to: Implement an Expression Tree Visitor](https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2008/bb882521(v=vs.90)?redirectedfrom=MSDN)

[Jaco Pretorius - Implementing a custom LINQ provider](https://jacopretorius.net/2010/01/implementing-a-custom-linq-provider.html)

[The Wayward WebLog - LINQ: Building an IQueryable Provider - Part I](https://learn.microsoft.com/en-us/archive/blogs/mattwar/linq-building-an-iqueryable-provider-part-i)

[The Wayward WebLog - LINQ: Building an IQueryable Provider - Part II](https://learn.microsoft.com/en-us/archive/blogs/mattwar/linq-building-an-iqueryable-provider-part-ii)

[Dixin's Blog - Understanding LINQ to SQL (10) Implementing LINQ to SQL Provider](https://weblogs.asp.net/dixin/understanding-linq-to-sql-10-implementing-linq-to-sql-provider)

[Sample CSV data sets](https://www.datablist.com/learn/csv/download-sample-csv-files)
