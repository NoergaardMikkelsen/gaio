using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ArtificialIntelligencePage
{
    private class
        ArtificialIntelligencePageUi : BaseUi<ArtificialIntelligencePageLogic, ArtificialIntelligenceViewModel>
    {
        public ArtificialIntelligencePageUi(
            ArtificialIntelligencePageLogic logic, ArtificialIntelligenceViewModel dataContext) : base(logic,
            dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            StackPanel buttonPanel = CreateButtonsPanel().Grid(row: 0, column: 0, columnSpan: 2);
            StackPanel inputPanel = CreateInputPanel().Grid(row: 1, column: 0, columnSpan: 5);
            DataGrid aiDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(ArtificialIntelligenceViewModel.ArtificialIntelligences), SetupDataGridColumns)
                .Grid(row: 2, column: 0, columnSpan: 5);

            StackPanel updatingPanel = CreateUpdatingTextBlock().Grid(row: 3, column: 0);
            StackPanel refreshButtons = CreateRefreshButtonsPanel(() => Logic.UpdateArtificialIntelligences())
                .Grid(row: 3, column: 4);


            grid.Children.Add(buttonPanel);
            grid.Children.Add(inputPanel);
            grid.Children.Add(aiDataGrid);
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

            StackPanel namePanel = StackPanelFactory.CreateLabeledFieldPanel("Name:",
                "Limit data grids content by content of 'Name'...",
                nameof(ArtificialIntelligenceViewModel.SearchableAiName));
            ViewModel.SearchableAiNameChanged += Logic.SearchFieldChanged;

            StackPanel keyPanel = StackPanelFactory.CreateLabeledFieldPanel("Key:",
                "Limit data grids content by content of 'Key'...",
                nameof(ArtificialIntelligenceViewModel.SearchableAiKey));
            ViewModel.SearchableAiKeyChanged += Logic.SearchFieldChanged;

            stackPanel.Children.Add(namePanel);
            stackPanel.Children.Add(keyPanel);

            return stackPanel;
        }

        private StackPanel CreateButtonsPanel()
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            var addButton = new Button()
            {
                Content = "Add",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            addButton.Click += Logic.AddButtonOnClick;
            stackPanel.Children.Add(addButton);

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
                DataGridColumns.NAME => nameof(ArtificialIntelligence.Name),
                DataGridColumns.KEY => nameof(ArtificialIntelligence.Key),
                DataGridColumns.AI_TYPE => nameof(ArtificialIntelligence.AiType),
                DataGridColumns.CREATED_AT => nameof(ArtificialIntelligence.CreatedDateTime),
                DataGridColumns.LAST_UPDATED_AT => nameof(ArtificialIntelligence.UpdatedDateTime),
                DataGridColumns.ACTIONS => nameof(ArtificialIntelligence.Id),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
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
                DataGridColumns.NAME => 50,
                DataGridColumns.KEY => 100,
                DataGridColumns.AI_TYPE => 25,
                DataGridColumns.CREATED_AT => 35,
                DataGridColumns.LAST_UPDATED_AT => 35,
                DataGridColumns.ACTIONS => 35,
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
                DataGridColumns.AI_TYPE => new EnumToTitleCaseConverter(),
                var _ => null,
            };
        }

        private FrameworkElement BuildActionsElement(int columnEnumAsInt)
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            var editButton = new Button()
            {
                Content = "Edit",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            editButton.Click += Logic.EditButtonOnClick;
            editButton.Tag(x => x.Binding(nameof(ArtificialIntelligence.Id)));
            var deleteButton = new Button()
            {
                Content = "Delete",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            deleteButton.Click += Logic.DeleteButtonOnClick;
            deleteButton.Tag(x => x.Binding(nameof(ArtificialIntelligence.Id)));

            stackPanel.Children.Add(editButton);
            stackPanel.Children.Add(deleteButton);

            return stackPanel;
        }
    }
}
