using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Models;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class AppliedKeywordsPage
{
    private class AppliedKeywordsPageUi : BaseUi<AppliedKeywordsPageLogic, AppliedKeywordsViewModel>
    {
        public AppliedKeywordsPageUi(AppliedKeywordsPageLogic logic, AppliedKeywordsViewModel dataContext) : base(logic,
            dataContext)
        {
        }

        protected override void AddControlsToGrid(Grid grid)
        {
            ComboBox aiSelectionComboBox = ComboBoxFactory
                .CreateAiSelectionComboBox(nameof(AppliedKeywordsViewModel.SelectedAiType)).Grid(row: 0, column: 4);
            DataGrid appliedKeywordsDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(AppliedKeywordsViewModel.AppliedKeywords), SetupDataGridColumns, Logic.SortItems)
                .Grid(row: 1, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel().Grid(row: 2, column: 4);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(appliedKeywordsDataGrid);
            grid.Children.Add(refreshButtons);
        }

        protected override StackPanel CreateRefreshButtonsPanel(Func<Task>? refreshAction = null)
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Right;

            var forceButton = new Button
            {
                Content = "Force Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            forceButton.Click += async (_, _) => await Logic.UpdateDisplayedItems(true);

            var button = new Button
            {
                Content = "Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            button.Click += async (_, _) => await Logic.UpdateDisplayedItems();

            stackPanel.Children.Add(forceButton);
            stackPanel.Children.Add(button);

            return stackPanel;
        }

        protected override void ConfigureGrid(Grid grid)
        {
            const int rowOneHeight = 8;
            const int rowThreeHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight - rowThreeHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Auto),
                new GridLength(rowTwoHeight, GridUnitType.Star), new GridLength(rowThreeHeight, GridUnitType.Auto));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private string GetColumnBindingPath(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.TEXT => nameof(AppliedKeyword.Text),
                DataGridColumns.USES_REGULAR_EXPRESSION => nameof(AppliedKeyword.UsesRegex),
                DataGridColumns.TOTAL_RESPONSES_COUNT => nameof(AppliedKeyword.TotalResponsesCount),
                DataGridColumns.MATCHING_RESPONSES_COUNT => nameof(AppliedKeyword.MatchingResponsesCount),
                DataGridColumns.START_SEARCH => nameof(AppliedKeyword.StartSearch),
                DataGridColumns.END_SEARCH => nameof(AppliedKeyword.EndSearch),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(new SetupColumnsArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetColumnBindingPath, GetEnumAsString,
                GetColumnStarWidth, GetValueConverterForColumn));
        }

        private string GetEnumAsString(int i)
        {
            return ((DataGridColumns) i).ToString();
        }

        private int GetColumnStarWidth(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.TEXT => 80,
                DataGridColumns.USES_REGULAR_EXPRESSION => 25,
                DataGridColumns.TOTAL_RESPONSES_COUNT => 35,
                DataGridColumns.MATCHING_RESPONSES_COUNT => 35,
                DataGridColumns.START_SEARCH => 40,
                DataGridColumns.END_SEARCH => 40,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.USES_REGULAR_EXPRESSION => new BooleanToYesNoConverter(),
                DataGridColumns.START_SEARCH => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.END_SEARCH => new UtcDateTimeToLocalStringConverter(),
                var _ => null,
            };
        }
    }
}
