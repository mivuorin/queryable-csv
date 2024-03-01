namespace QueryableCsv.Test;

public class QueryableTest
{
    private Person _jane;
    private Person _john;
    private Person _tom;
    private QueryableCsv<Person> _queryable;

    [SetUp]
    public void SetUp()
    {
        _tom = new Person
        {
            Id = 1,
            Name = "Tom",
            BirthDay = new DateTime(1982, 6, 11, 0, 0, 0).ToLocalTime()
        };

        _jane = new Person
        {
            Id = 2, Name = "Jane", BirthDay = new DateTime(2016, 5, 14, 13, 15, 0).ToLocalTime()
        };

        _john = new Person
        {
            Id = 3, Name = "John", BirthDay = new DateTime(1962, 3, 1, 0, 0, 0).ToLocalTime()
        };

        _queryable = new QueryableCsv<Person>("persons.csv");
    }

    [Test]
    public void Implements_queryable()
    {
        var expected = new[]
        {
            _jane
        };

        var query = from p in _queryable
            where p.Id == 2
            select p;

        var actual = query.ToList();

        actual.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Test]
    public void Duplicate_where_query()
    {
        var expected = new[]
        {
            _john
        };

        var query = from p in _queryable
            where p.Id > 1
            where p.Name.StartsWith("Jo")
            select p;

        var actual = query.ToList();

        actual.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Test]
    public void Support_projection()
    {
        var expected = new[] { new { _tom.Name } };

        var query = from p in _queryable
            where p.Id == 1
            select new { p.Name };

        var actual = query.ToList();

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Implements_single()
    {
        var query = from p in _queryable
            where p.Id == 1
            select p;

        var actual = query.Single();

        actual.Should().BeEquivalentTo(_tom);
    }

    [Test]
    public void Implements_count()
    {
        var query = from p in _queryable
            where p.Id == 1
            select p;

        var actual = query.Count();

        actual.Should().Be(1);
    }

    [Test]
    public void Execute_query_multiple_times()
    {
        var query = from p in _queryable
            select p;

        var first = query.Count();

        var secondQuery = from p in query
            where p.Id == 1
            select p;

        var second = secondQuery.Single();

        first.Should().Be(3);
        second.Should().BeEquivalentTo(_tom);
    }

    [Test]
    public void Indexed_where_query()
    {
        var actual = _queryable.Where((p, i) => i == 2).Single();
        actual.Should().BeEquivalentTo(_john);
    }
}