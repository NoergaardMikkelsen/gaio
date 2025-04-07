using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Extensions;

namespace Statistics.Uno.Presentation.Factory;

public static class ComboBoxFactory
{
    public static ComboBox CreateAiSelectionComboBox(SelectionChangedEventHandler selectionChangedHandler)
    {
        var comboBox = new ComboBox()
        {
            Margin = new Thickness(10),
            BorderBrush = new SolidColorBrush(Colors.White),
        };

        var options = typeof(ArtificialIntelligenceType).EnumNamesToTitleCase();

        comboBox.ItemsSource = options;
        comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;
        comboBox.SelectionChanged += selectionChangedHandler;

        return comboBox;
    }
}
