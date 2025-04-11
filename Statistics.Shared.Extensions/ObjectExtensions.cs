using System.Reflection;

namespace Statistics.Shared.Extensions;

public static class ObjectExtensions
{
    public static object? GetSortableValue(this object obj, string propertyName)
    {
        PropertyInfo? property = obj.GetType().GetProperty(propertyName);
        if (property == null)
        {
            return null;
        }

        object? value = property.GetValue(obj);

        // Handle null values
        if (value == null)
        {
            return null;
        }

        // Handle boolean values
        if (value is bool boolValue)
        {
            return boolValue ? 1 : 0; // Convert boolean to numeric for sorting
        }

        // Handle numeric values
        if (value is IComparable comparableValue)
        {
            return comparableValue;
        }

        // Handle DateTime values
        if (value is DateTime dateTimeValue)
        {
            return dateTimeValue.Ticks; // Use Ticks for consistent sorting
        }

        // Handle text values (default case)
        return value.ToString();
    }
}
