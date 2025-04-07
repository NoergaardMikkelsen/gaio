using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage : BasePage
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        RESPONSE_TEXT = 1,
        CREATED_AT = 2,
    }

    public ResponsesPage()
    {
        IResponseEndpoint responseApi = GetResponseApi();
        IActionEndpoint actionApi = GetActionApi();

        DataContext = new ResponsesViewModel();

        var logic = new ResponsesPageLogic(responseApi, actionApi, (ResponsesViewModel) DataContext);
        var ui = new ResponsesPageUi(logic, (ResponsesViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateResponses();
    }

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
                .CreateDataGrid(ViewModel, nameof(ResponsesViewModel.Responses), SetupDataGridColumns)
                .Grid(row: 2, column: 0, columnSpan: 5);
            StackPanel refreshButtons =
                CreateRefreshButtonsPanel(() => Logic.UpdateResponses()).Grid(row: 3, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(inputPanel);
            grid.Children.Add(responsesDataGrid);
            grid.Children.Add(refreshButtons);
        }

        private StackPanel CreateInputPanel()
        {
            StackPanel stackPanel = StackPanelFactory.CreateDefaultPanel();

            StackPanel textPanel = StackPanelFactory.CreateLabeledFieldPanel("Response Text:",
                "Limit data grids content by content of 'Response Text'...",
                nameof(ResponsesViewModel.SearchableResponseText));
            ViewModel.SearchableResponseTextChanged += Logic.SearchFieldChanged;

            stackPanel.Children.Add(textPanel);

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
                DataGridColumns.PROMPT_TEXT => 100,
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

    private class ResponsesPageLogic
    {
        private readonly IResponseEndpoint responseApi;
        private readonly IActionEndpoint actionApi;
        private ResponsesViewModel ViewModel { get; }

        public ResponsesPageLogic(IResponseEndpoint responseApi, IActionEndpoint actionApi, ResponsesViewModel dataContext)
        {
            this.responseApi = responseApi;
            this.actionApi = actionApi;
            ViewModel = dataContext;
            ViewModel.Responses = new List<IResponse>();
        }

        internal async Task UpdateResponses()
        {
            IComplexSearchable complexSearchable = BuildComplexSearchable();

            var apiResponse = await responseApi.GetAllByComplexQuery(CancellationToken.None, (ComplexSearchable) complexSearchable);

            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                return;
            }

            List<Response> responses = apiResponse.Content;

            if (responses == null)
            {
                Console.WriteLine($"Failed to get selected Responses entities.");
                return;
            }

            ViewModel.Responses = responses;
        }

        private IComplexSearchable BuildComplexSearchable()
        {
            return new ComplexSearchable()
            {
                SearchableResponse = new SearchableResponse(), SearchableArtificialIntelligence =
                    new SearchableArtificialIntelligence()
                    {
                        AiType = ViewModel.SelectedAiType,
                    },
            };
        }

        public async void ExecuteAllPromptsOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            ViewModel.ExecuteAllPromptsButtonText = "Executing...";
            button.IsEnabled = false;
            await actionApi.ExecuteAllPrompts(CancellationToken.None);
            ViewModel.ExecuteAllPromptsButtonText = "Execute Prompts";
            button.IsEnabled = true;
            await UpdateResponses();
        }

        public async void SearchFieldChanged(object? sender, string e)
        {
            if (e.Length < 5)
            {
                return;
            }

            await UpdateResponses();
        }
    }
}
