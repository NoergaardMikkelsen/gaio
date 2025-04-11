using Statistics.Shared.Abstraction.Enum;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.Factory;

public static class ComboBoxFactory
{
    public static ComboBox CreateAiSelectionComboBox(string bindingPath)
    {
        var comboBox = new ComboBox
        {
            Margin = new Thickness(10),
            BorderBrush = new SolidColorBrush(Colors.White),
        };

        var converter = new ArtificialIntelligenceTypeToHumanReadableConverter();
        // Create a collection of human-readable strings for the ComboBox items
        var aiTypes = Enum.GetValues<ArtificialIntelligenceType>()
            .Select(aiType => converter.Convert(aiType, typeof(string), null, null)).ToList();

        comboBox.ItemsSource = aiTypes;

        var indexBinding = new Binding
        {
            Mode = BindingMode.TwoWay,
            Path = new PropertyPath(bindingPath),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new EnumToIntConverter(),
        };
        comboBox.SetBinding(ComboBox.SelectedIndexProperty, indexBinding);

        return comboBox;
    }
}
