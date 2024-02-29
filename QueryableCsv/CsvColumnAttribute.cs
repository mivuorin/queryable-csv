namespace QueryableCsv;

[AttributeUsage(AttributeTargets.Property)]
public class CsvColumnAttribute : Attribute
{
    /// <summary>
    /// Marks property to be mapped given column in csv file.
    /// </summary>
    /// <param name="index">Zero based index of mapped column in csv file.</param>
    public CsvColumnAttribute(int index)
    {
        Index = index;
    }

    public int Index { get; }
}