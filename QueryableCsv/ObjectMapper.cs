using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace QueryableCsv;

internal class ObjectMapper
{
    private readonly Type _objectType;
    private readonly IEnumerable<(PropertyInfo p, CsvColumnAttribute)> _propertyColumnMappings;
    private readonly char _separator;

    internal ObjectMapper(Type objectType, char separator)
    {
        _objectType = objectType;
        _separator = separator;
        _propertyColumnMappings = _objectType.GetProperties()
            .Where(p => p.GetCustomAttributes<CsvColumnAttribute>().Any())
            .Select(p => (p, p.GetCustomAttributes<CsvColumnAttribute>().Single()));
    }

    internal IEnumerable ReadFile(FilterVisitor visitor, TextReader reader)
    {
        // TODO Generic collection might not be needed. Depends on how to convert this into IEnumerator
        var collectionType = typeof(List<>).MakeGenericType(_objectType);
        var collection = (IList)Activator.CreateInstance(collectionType)!;

        while (reader.ReadLine() is string line)
        {
            var columns = line.Split(_separator);
            var instance = Activator.CreateInstance(_objectType);

            foreach (var (property, columnAttribute) in _propertyColumnMappings)
            {
                // TODO Provide helpful parsing error messages with line number, property name and column index
                var parse = Parser(property.PropertyType);
                var input = columns[columnAttribute.Index];
                var value = parse(input);
                property.SetValue(instance, value);
            }

            if (visitor.Filter != null)
            {
                // TODO Is it possible to add type safety? Cannot apply Linq operators can oly be applied to IEnumerable<T>. 
                if ((bool)visitor.Filter.DynamicInvoke(instance)!)
                {
                    collection.Add(instance);
                }
            }
            else
            {
                collection.Add(instance);
            }
        }

        return collection;
    }

    private static Func<string, object> Parser(Type type)
    {
        // TODO Clean up parser
        var converter = TypeDescriptor.GetConverter(type);
        return input => converter.ConvertFromString(null, CultureInfo.InvariantCulture, input);
    }
}