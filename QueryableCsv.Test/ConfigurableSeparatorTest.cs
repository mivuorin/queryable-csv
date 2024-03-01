namespace QueryableCsv.Test;

[TestFixture]
public class ConfigurableSeparatorTest
{
    private class TestObject
    {
        [CsvColumn(0)]
        public int Id { get; set; }

        [CsvColumn(1)]
        public string Text { get; set; } = "";
    }

    [Test]
    public void Comma_is_default_separator()
    {
        using var reader = new StringReader("1,text");
        var queryable = new QueryableCsv<TestObject>(reader);

        var actual = queryable.Single();

        var expected = new TestObject
        {
            Id = 1,
            Text = "text"
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Configurable_separator()
    {
        using var reader = new StringReader("1|text");
        var queryable = new QueryableCsv<TestObject>(reader, '|');

        var actual = queryable.Single();

        var expected = new TestObject
        {
            Id = 1,
            Text = "text"
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Configurable_separator_with_path()
    {
        var queryable = new QueryableCsv<Person>("persons.csv", ',');

        var actual = queryable.First();
        
        actual.Id.Should().Be(1);
    }
}