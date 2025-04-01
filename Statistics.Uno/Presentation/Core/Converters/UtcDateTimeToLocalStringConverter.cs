using System.Globalization;

namespace Statistics.Uno.Presentation.Core.Converters;

public class UtcDateTimeToLocalStringConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string dateString && DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            return dateTime.ToUniversalTime();
        }

        return DependencyProperty.UnsetValue;
    }
}
