using System.Text;

namespace QueryableCsv.Test;

public class ObjectMappingTest
{
    private class NumericTypes
    {
        [CsvColumn(0)]
        public uint UShort { get; set; }

        [CsvColumn(1)]
        public short Short { get; set; }

        [CsvColumn(2)]
        public uint UInteger { get; set; }

        [CsvColumn(3)]
        public int Integer { get; set; }

        [CsvColumn(4)]
        public float Float { get; set; }

        [CsvColumn(5)]
        public double Double { get; set; }

        [CsvColumn(6)]
        public long Long { get; set; }

        [CsvColumn(7)]
        public ulong ULong { get; set; }

        [CsvColumn(8)]
        public decimal Decimal { get; set; }
    }

    [Test]
    public void Numeric_types()
    {
        var expected = new NumericTypes
        {
            UShort = ushort.MaxValue,
            Short = short.MinValue,
            UInteger = uint.MaxValue,
            Integer = int.MinValue,
            Float = float.MinValue,
            Double = -1.05e+003,
            Long = long.MaxValue,
            ULong = ulong.MaxValue,
            Decimal = decimal.MaxValue
        };

        using var reader =
            new StringReader(
                "65535,-32768,4294967295,-2147483648,-3.4028235E+38,-1.05e+003,9223372036854775807,18446744073709551615,79228162514264337593543950335");
        var queryable = new QueryableCsv<NumericTypes>(reader);

        var actual = queryable.Single();
        actual.Should().BeEquivalentTo(expected);
    }

    private class StringAndChar
    {
        [CsvColumn(0)]
        public string String { get; set; } = "";

        [CsvColumn(1)]
        public string? NullableString { get; set; }

        [CsvColumn(2)]
        public char Char { get; set; }
    }

    [Test]
    public void String_and_char()
    {
        using var reader = new StringReader("string,,c");
        var queryable = new QueryableCsv<StringAndChar>(reader);

        var actual = queryable.Single();

        var expected = new StringAndChar
        {
            String = "string",
            NullableString = "", // TODO Separate empty string from null if string is nullable?
            Char = 'c'
        };

        actual.Should().BeEquivalentTo(expected);
    }

    private class Bytes
    {
        [CsvColumn(0)]
        public byte Byte { get; set; }

        [CsvColumn(1)]
        public sbyte SignedByte { get; set; }
    }

    [Test]
    public void Byte_types()
    {
        using var reader = new StringReader("0xFF,0x80");
        var queryable = new QueryableCsv<Bytes>(reader);

        var actual = queryable.Single();

        var expected = new Bytes
        {
            Byte = 255,
            SignedByte = -128
        };

        actual.Should().BeEquivalentTo(expected);
    }

    private class BooleanType
    {
        [CsvColumn(0)]
        public bool Bool { get; set; }
    }

    [Test]
    public void Boolean_type()
    {
        var sb = new StringBuilder();
        sb.AppendLine("False");
        sb.AppendLine("tRue");
        sb.AppendLine("false");
        sb.AppendLine("TRUE");
        sb.AppendLine("falsE");

        using var reader = new StringReader(sb.ToString());
        var queryable = new QueryableCsv<BooleanType>(reader);

        var actual = queryable.Select(b => b.Bool).ToArray();

        actual[0].Should().BeFalse();
        actual[1].Should().BeTrue();
        actual[2].Should().BeFalse();
        actual[3].Should().BeTrue();
        actual[4].Should().BeFalse();
    }

    private class DateTimeType
    {
        [CsvColumn(0)]
        public DateTime DateTime { get; set; }
    }

    [Test]
    public void DateTime_in_iso_format()
    {
        using var reader = new StringReader("2024-02-29T14:35:10.452Z");
        var queryable = new QueryableCsv<DateTimeType>(reader);

        var actual = queryable.Single();

        var expected = new DateTime(2024, 2, 29, 14, 35, 10, 452).ToLocalTime();
        actual.DateTime.Should().Be(expected);
        actual.DateTime.Kind.Should().Be(DateTimeKind.Local);
    }
}