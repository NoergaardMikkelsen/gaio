using Statistics.Shared.Abstraction.Enum;

namespace Statistics.Uno.Presentation.Core.Converters;

public class ArtificialIntelligenceTypeToHumanReadableConverter : IValueConverter
{
    private readonly Dictionary<ArtificialIntelligenceType, string> typeMap = new()
    {
        {ArtificialIntelligenceType.OPEN_AI_NO_WEB, "Open Ai - Without Web"},
        {ArtificialIntelligenceType.OPEN_AI_WEB, "Open Ai - With Web"},
    };

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || !value.GetType().IsEnum || value is not ArtificialIntelligenceType aiType)
        {
            return DependencyProperty.UnsetValue;
        }

        if (typeMap.TryGetValue(aiType, out string? humanReadable))
        {
            return humanReadable;
        }

        return DependencyProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value == null || targetType == null || !targetType.IsEnum || value is not string humanReadableString)
        {
            return DependencyProperty.UnsetValue;
        }

        var matchingEntry =
            typeMap.FirstOrDefault(kvp => kvp.Value.Equals(humanReadableString, StringComparison.OrdinalIgnoreCase));
        if (!matchingEntry.Equals(default(KeyValuePair<ArtificialIntelligenceType, string>)))
        {
            return matchingEntry.Key;
        }

        return DependencyProperty.UnsetValue;
    }
}
