using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Extensions;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.Factory;

public static class ComboBoxFactory
{
    public static ComboBox CreateAiSelectionComboBox(string bindingPath)
    {
        var comboBox = new ComboBox()
        {
            Margin = new Thickness(10),
            BorderBrush = new SolidColorBrush(Colors.White),
        };

        var options = typeof(ArtificialIntelligenceType).EnumNamesToTitleCase();
        comboBox.ItemsSource = options;

        var binding = new Binding()
        {
            Mode = BindingMode.TwoWay,
            Path = new PropertyPath(bindingPath),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new EnumToIntConverter(),
        };
        comboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

        return comboBox;
    }
}
