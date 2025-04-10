using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage
{
    private class ResponsesPageUi : BaseUi<ResponsesPageLogic, ResponsesViewModel>
    {
        public ResponsesPageUi(ResponsesPageLogic logic, ResponsesViewModel dataContext) : base(logic, dataContext)
        {
        }

        protected override void AddControlsToGrid(Grid grid)
        {
            StackPanel buttonPanel = CreateButtonsPanel().Grid(row: 0, column: 0, columnSpan: 2);
            ComboBox aiSelectionComboBox = ComboBoxFactory.CreateAiSelectionComboBox(nameof(ResponsesViewModel.SelectedAiType))
                .Grid(row: 0, column: 4);
            StackPanel inputPanel = CreateInputPanel().Grid(row: 1, column: 0, columnSpan: 5);
            DataGrid responsesDataGrid = DataGridFactory
                .CreateDataGrid(ViewModel, nameof(ResponsesViewModel.Responses), SetupDataGridColumns, Logic.SortItems)
                .Grid(row: 2, column: 0, columnSpan: 5);
            StackPanel refreshButtons =
                CreateRefreshButtonsPanel(() => Logic.UpdateDisplayedItems()).Grid(row: 3, column: 4);
            StackPanel updatingPanel = CreateUpdatingTextBlock().Grid(row: 3, column: 0);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(inputPanel);
            grid.Children.Add(responsesDataGrid);
            grid.Children.Add(updatingPanel);
            grid.Children.Add(refreshButtons);
        }

        private StackPanel CreateUpdatingTextBlock()
        {
            var panel = StackPanelFactory.CreateDefaultPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Left;

            var block = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5),
                Width = 200,
            };
            block.Text(() => ViewModel.UpdatingText);

            panel.Children.Add(block);

            return panel;
        }

        private StackPanel CreateInputPanel()
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            StackPanel promptTextPanel = StackPanelFactory.CreateLabeledFieldPanel("Prompt Text:",
                "Limit data grids content by content of 'Prompt Text'...",
                nameof(ResponsesViewModel.SearchablePromptText));
            ViewModel.SearchablePromptTextChanged += Logic.SearchFieldChanged;

            StackPanel responseTextPanel = StackPanelFactory.CreateLabeledFieldPanel("Response Text:",
                "Limit data grids content by content of 'Response Text'...",
                nameof(ResponsesViewModel.SearchableResponseText));
            ViewModel.SearchableResponseTextChanged += Logic.SearchFieldChanged;

            stackPanel.Children.Add(promptTextPanel);
            stackPanel.Children.Add(responseTextPanel);

            return stackPanel;
        }

        private StackPanel CreateButtonsPanel()
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            var executeAllPromptsButton = new Button()
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            executeAllPromptsButton.Content(() => ViewModel.ExecuteAllPromptsButtonText);
            executeAllPromptsButton.Click += Logic.ExecuteAllPromptsOnClick;

            stackPanel.Children.Add(executeAllPromptsButton);

            return stackPanel;
        }

        protected override void ConfigureGrid(Grid grid)
        {
            const int rowOneHeight = 10;
            const int rowTwoHeight = 10;
            const int rowFourHeight = 10;
            const int rowThreeHeight = 100 - rowOneHeight - rowTwoHeight - rowFourHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Auto),
                new GridLength(rowTwoHeight, GridUnitType.Auto), new GridLength(rowThreeHeight, GridUnitType.Star),
                new GridLength(rowFourHeight, GridUnitType.Auto));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private string GetColumnBindingPath(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.PROMPT_TEXT => $"{nameof(Response.Prompt)}.{nameof(Prompt.Text)}",
                DataGridColumns.RESPONSE_TEXT => nameof(Response.Text),
                DataGridColumns.CREATED_AT => nameof(Response.CreatedDateTime),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(new SetupColumnsArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetColumnBindingPath, GetEnumAsString,
                GetColumnStarWidth, GetValueConverterForColumn));
        }

        private string GetEnumAsString(int columnNumber)
        {
            return ((DataGridColumns) columnNumber).ToString();
        }

        private int GetColumnStarWidth(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.PROMPT_TEXT => 70,
                DataGridColumns.RESPONSE_TEXT => 100,
                DataGridColumns.CREATED_AT => 35,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.CREATED_AT => new UtcDateTimeToLocalStringConverter(),
                var _ => null,
            };
        }
    }
}
