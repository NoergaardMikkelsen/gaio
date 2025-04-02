using Microsoft.UI.Xaml.Data;

namespace Statistics.Uno.Presentation.Core.Converters;

public class BooleanToYesNoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? "Yes" : "No";
        }

        throw new ArgumentException("Value must be a boolean");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue)
        {
            return stringValue.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        }

        throw new ArgumentException("Value must be a string");
    }
}

