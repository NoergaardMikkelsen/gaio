using System;
using System.Globalization;
using System.Linq;
using Statistics.Shared.Extensions;

namespace Statistics.Uno.Presentation.Core.Converters
{

    public class EnumToTitleCaseConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !value.GetType().IsEnum)
            {
                return DependencyProperty.UnsetValue;
            }

            string enumString = value.ToString();
            return enumString.ScreamingSnakeCaseToTitleCase();
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || targetType == null || !targetType.IsEnum)
            {
                return DependencyProperty.UnsetValue;
            }

            string titleCaseString = value.ToString();
            string screamingSnakeCaseString =
                string.Join('_', titleCaseString.Split(' ').Select(word => word.ToUpper()));

            if (Enum.TryParse(targetType, screamingSnakeCaseString, out var enumValue))
            {
                return enumValue;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
