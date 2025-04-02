using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage : Page
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        RESPONSE_TEXT = 1,
        CREATED_AT = 2,
        LAST_UPDATED_AT = 3,
    }

    public ResponsesPage()
    {
        var app = (App) Application.Current;

        IArtificialIntelligenceEndpoint aiApi =
            app.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
            throw new NullReferenceException(
                $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");

        DataContext = new ResponsesViewModel();

        var logic = new ResponsesPageLogic(aiApi, (ResponsesViewModel) DataContext);
        var ui = new ResponsesPageUi(logic, (ResponsesViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdateResponses();
    }

    private class ResponsesPageUi : BaseUi<ResponsesPageLogic, ResponsesViewModel>
    {
        public ResponsesPageUi(ResponsesPageLogic logic, ResponsesViewModel dataContext) : base(logic, dataContext)
        {
        }

        protected override void AddControlsToGrid(Grid grid)
        {
            ComboBox aiSelectionComboBox = CreateAiSelectionComboBox().Grid(row: 0, column: 4);
            DataGrid responsesDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(ResponsesViewModel.Responses), SetupDataGridColumns, SetupDataGridRowTemplate)
                .Grid(row: 1, column: 0, columnSpan: 5);
            Button refreshButton = CreateRefreshButton().Grid(row: 2, column: 4);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(responsesDataGrid);
            grid.Children.Add(refreshButton);
        }

        private Button CreateRefreshButton()
        {
            var button = new Button()
            {
                Content = "Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            button.Click += async (_, _) => await Logic.UpdateResponses();
            return button;
        }

        protected override void ConfigureGrid(Grid grid)
        {
            const int rowOneHeight = 8;
            const int rowThreeHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight - rowThreeHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star), new GridLength(rowThreeHeight, GridUnitType.Star));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridRowTemplate(new SetupRowArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetValueConverterForColumn,
                GetColumnBindingPath));
        }

        private string GetColumnBindingPath(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.PROMPT_TEXT => $"{nameof(Response.Prompt)}.{nameof(Prompt.Text)}",
                DataGridColumns.RESPONSE_TEXT => $"{nameof(Response.Text)}",
                DataGridColumns.CREATED_AT => $"{nameof(Response.CreatedDateTime)}",
                DataGridColumns.LAST_UPDATED_AT => $"{nameof(Response.UpdatedDateTime)}",
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(new SetupColumnsArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetColumnBindingPath,IsDateColumn, GetEnumAsString, GetColumnStarWidth, GetValueConverterForColumn));
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
                DataGridColumns.PROMPT_TEXT => 100,
                DataGridColumns.RESPONSE_TEXT => 100,
                DataGridColumns.CREATED_AT => 35,
                DataGridColumns.LAST_UPDATED_AT => 35,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private bool IsDateColumn(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.CREATED_AT => true,
                DataGridColumns.LAST_UPDATED_AT => true,
                _ => false,
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.CREATED_AT => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.LAST_UPDATED_AT => new UtcDateTimeToLocalStringConverter(),
                _ => null,
            };
        }

        private ComboBox CreateAiSelectionComboBox()
        {
            var comboBox = new ComboBox()
            {
                Margin = new Thickness(10), BorderBrush = new SolidColorBrush(Colors.White),
            };

            var options = typeof(ArtificialIntelligenceType).EnumNamesToTitleCase();

            comboBox.ItemsSource = options;
            comboBox.SelectionChanged += Logic.ComboBoxOnSelectionChanged;
            comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;

            return comboBox;
        }
    }

    private class ResponsesPageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private ResponsesViewModel ViewModel { get; }

        public ResponsesPageLogic(IArtificialIntelligenceEndpoint aiApi, ResponsesViewModel dataContext)
        {
            this.aiApi = aiApi;
            ViewModel = dataContext;
            ViewModel.Responses = new List<IResponse>();
        }

        public void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox ??
                                throw new NullReferenceException(
                                    $"Expected '{nameof(sender)}' to not be null, but it was.");
            comboBoxSelection = (ArtificialIntelligenceType) comboBox.SelectedIndex;
        }

        internal async Task UpdateResponses()
        {
            var apiResponse = await aiApi.GetByQuery(CancellationToken.None,
                new SearchableArtificialIntelligence() {AiType = comboBoxSelection,});
            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            ArtificialIntelligence? selectedAiEntity = apiResponse.Content;

            if (selectedAiEntity == null)
            {
                Console.WriteLine($"Failed to get selected Artificial Intelligence entity.");
                return;
            }

            ViewModel.Responses = selectedAiEntity.Responses.ToList();
        }
    }
}
