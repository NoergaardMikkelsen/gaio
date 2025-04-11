using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class PromptsPage
{
    private class PromptsPageUi : BaseUi<PromptsPageLogic, PromptsViewModel>
    {
        public PromptsPageUi(PromptsPageLogic logic, PromptsViewModel dataContext) : base(logic, dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            StackPanel buttonPanel = CreateButtonsPanel().Grid(row: 0, column: 0, columnSpan: 2);
            StackPanel inputPanel = CreateInputPanel().Grid(row: 1, column: 0, columnSpan: 5);
            DataGrid promptsDataGrid = DataGridFactory
                .CreateDataGrid(ViewModel, nameof(PromptsViewModel.Prompts), SetupDataGridColumns, Logic.SortItems)
                .Grid(row: 2, column: 0, columnSpan: 5);
            StackPanel updatingPanel = CreateUpdatingTextBlock().Grid(row: 3, column: 0);
            StackPanel refreshButtons =
                CreateRefreshButtonsPanel(() => Logic.UpdateDisplayedItems()).Grid(row: 3, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(inputPanel);
            grid.Children.Add(promptsDataGrid);
            grid.Children.Add(updatingPanel);
            grid.Children.Add(refreshButtons);
        }

        private StackPanel CreateUpdatingTextBlock()
        {
            StackPanel? panel = StackPanelFactory.CreateDefaultPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Left;

            var block = new TextBlock
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

            StackPanel textPanel = StackPanelFactory.CreateLabeledFieldPanel("Prompt Text:",
                "Limit data grids content by content of 'Prompt Text'...",
                nameof(PromptsViewModel.SearchablePromptText));
            ViewModel.SearchablePromptTextChanged += Logic.SearchFieldChanged;

            stackPanel.Children.Add(textPanel);

            return stackPanel;
        }

        private StackPanel CreateButtonsPanel()
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            var addButton = new Button
            {
                Content = "Add",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            addButton.Click += Logic.AddButtonOnClick;

            var executeAllPromptsButton = new Button
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            executeAllPromptsButton.Content(() => ViewModel.ExecuteAllPromptsButtonText);
            executeAllPromptsButton.Click += Logic.ExecuteAllPromptsOnClick;

            stackPanel.Children.Add(addButton);
            stackPanel.Children.Add(executeAllPromptsButton);

            return stackPanel;
        }

        /// <inheritdoc />
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
                DataGridColumns.PROMPT_TEXT => nameof(Prompt.Text),
                DataGridColumns.CREATED_AT => nameof(Prompt.CreatedDateTime),
                DataGridColumns.LAST_UPDATED_AT => nameof(Prompt.UpdatedDateTime),
                DataGridColumns.ACTIONS => nameof(Prompt.Id),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.CREATED_AT => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.LAST_UPDATED_AT => new UtcDateTimeToLocalStringConverter(),
                var _ => null,
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(new SetupColumnsArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetColumnBindingPath, GetEnumAsString,
                GetColumnStarWidth, GetValueConverterForColumn, BuildActionsElement));
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
                DataGridColumns.PROMPT_TEXT => 100,
                DataGridColumns.CREATED_AT => 25,
                DataGridColumns.LAST_UPDATED_AT => 25,
                DataGridColumns.ACTIONS => 35,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private FrameworkElement BuildActionsElement(int columnEnumAsInt)
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            var editButton = new Button
            {
                Content = "Edit",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            editButton.Click += Logic.EditButtonOnClick;
            editButton.Tag(x => x.Binding(nameof(Prompt.Id)));
            var deleteButton = new Button
            {
                Content = "Delete",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            deleteButton.Click += Logic.DeleteButtonOnClick;
            deleteButton.Tag(x => x.Binding(nameof(Prompt.Id)));

            var executeButton = new Button
            {
                Content = "Execute",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            executeButton.Click += Logic.ExecutePromptOnClick;
            executeButton.Tag(x => x.Binding(nameof(Prompt.Id)));

            stackPanel.Children.Add(editButton);
            stackPanel.Children.Add(deleteButton);
            stackPanel.Children.Add(executeButton);

            return stackPanel;
        }
    }
}
