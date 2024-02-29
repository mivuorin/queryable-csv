namespace QueryableCsv.Test;

public class Person
{
    [CsvColumn(0)]
    public int Id { get; set; }

    [CsvColumn(1)]
    public string Name { get; set; } = "";

    [CsvColumn(2)]
    public DateTime BirthDay { get; set; }
}