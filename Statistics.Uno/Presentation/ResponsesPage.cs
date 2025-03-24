using Statistics.Shared.Abstraction.Enum;

namespace Statistics.Uno.Presentation;

public sealed partial class ResponsesPage : Page
{
    public ResponsesPage()
    {
        var logic = new ResponsesPageLogic();
        var ui = new ResponsesPageUi(logic);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class ResponsesPageUi
    {
        private readonly ResponsesPageLogic logic;

        public ResponsesPageUi(ResponsesPageLogic logic)
        {
            this.logic = logic;
        }

        public Grid CreateContentGrid()
        {
            var grid = new Grid();

            const int rowOneHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star));

            var aiSelectionComboBox = CreateAiSelectionComboBox().Grid(row: 0, column: 0);

            grid.Children.Add(aiSelectionComboBox);

            return grid;
        }

        private ComboBox CreateAiSelectionComboBox()
        {
            var comboBox = new ComboBox();

            var options = Enum.GetNames(typeof(ArtificialIntelligenceType)).Select(x => x.Split('_')).Select(x =>
                Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(string.Join(' ', x)));

            comboBox.ItemsSource = options;
            comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;

            return comboBox;
        }
    }

    private class ResponsesPageLogic
    {

    }
}
