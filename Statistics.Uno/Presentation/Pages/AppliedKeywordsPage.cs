using System.Net;
using CommunityToolkit.WinUI.UI.Controls;
using Refit;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class AppliedKeywordsPage : Page
{
    private enum DataGridColumns
    {
        TEXT = 0,
        USES_REGEX = 1,
        MATCHING_RESPONSES_COUNT = 2,
        TOTAL_RESPONSES_COUNT = 3,
        START_SEARCH = 4,
        END_SEARCH = 5,
    }

    public AppliedKeywordsPage()
    {
        GetServices(out IAppliedKeywordService keywordService, out IResponsesEndpoint responsesApi, out IKeywordEndpoint keywordApi);

        DataContext = new AppliedKeywordsViewModel();

        var logic = new AppliedKeywordsPageLogic(keywordService, responsesApi, keywordApi, (AppliedKeywordsViewModel)DataContext);
        var ui = new AppliedKeywordsPageUi(logic, (AppliedKeywordsViewModel)DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateAppliedKeywords();
    }

    private void GetServices(
        out IAppliedKeywordService keywordService, out IResponsesEndpoint responsesApi, out IKeywordEndpoint keywordApi)
    {
        var app = (App) Application.Current;

        keywordService = app.Startup.ServiceProvider.GetService<IAppliedKeywordService>() ??
                         throw new NullReferenceException(
                             $"Failed to acquire an instance implementing '{nameof(IAppliedKeywordService)}'.");

        responsesApi = app.Startup.ServiceProvider.GetService<IResponsesEndpoint>() ??
                       throw new NullReferenceException(
                           $"Failed to acquire an instance implementing '{nameof(IResponsesEndpoint)}'.");

        keywordApi = app.Startup.ServiceProvider.GetService<IKeywordEndpoint>() ??
                     throw new NullReferenceException(
                         $"Failed to acquire an instance implementing '{nameof(IKeywordEndpoint)}'.");
    }

    private class AppliedKeywordsPageUi : BaseUi<AppliedKeywordsPageLogic, AppliedKeywordsViewModel>
    {
        public AppliedKeywordsPageUi(AppliedKeywordsPageLogic logic, AppliedKeywordsViewModel dataContext) : base(logic, dataContext)
        {
        }

        protected override void AddControlsToGrid(Grid grid)
        {
            ComboBox aiSelectionComboBox = ComboBoxFactory.CreateAiSelectionComboBox(Logic.ComboBoxOnSelectionChanged)
                .Grid(row: 0, column: 4);
            DataGrid appliedKeywordsDataGrid = DataGridFactory.CreateDataGrid(
                ViewModel, nameof(AppliedKeywordsViewModel.AppliedKeywords), SetupDataGridColumns,
                SetupDataGridRowTemplate).Grid(row: 1, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel().Grid(row: 2, column: 4);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(appliedKeywordsDataGrid);
            grid.Children.Add(refreshButtons);
        }

        protected override StackPanel CreateRefreshButtonsPanel(Func<Task>? refreshAction = null)
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10),
            };

            var forceButton = new Button()
            {
                Content = "Force Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            forceButton.Click += async (_, _) => await Logic.UpdateAppliedKeywords(true);

            var button = new Button()
            {
                Content = "Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            button.Click += async (_, _) => await Logic.UpdateAppliedKeywords();

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
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star), new GridLength(rowThreeHeight, GridUnitType.Star));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridRowTemplate(new SetupRowArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetValueConverterForColumn, GetColumnBindingPath));
        }

        private string GetColumnBindingPath(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.TEXT => nameof(AppliedKeyword.Text),
                DataGridColumns.USES_REGEX => nameof(AppliedKeyword.UsesRegex),
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
            return ((DataGridColumns)i).ToString();
        }

        private int GetColumnStarWidth(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.TEXT => 100,
                DataGridColumns.USES_REGEX => 15,
                DataGridColumns.TOTAL_RESPONSES_COUNT => 35,
                DataGridColumns.MATCHING_RESPONSES_COUNT => 35,
                DataGridColumns.START_SEARCH => 40,
                DataGridColumns.END_SEARCH => 40,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.USES_REGEX => new BooleanToYesNoConverter(),
                DataGridColumns.START_SEARCH => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.END_SEARCH => new UtcDateTimeToLocalStringConverter(),
                var _ => null,
            };
        }
    }

    private class AppliedKeywordsPageLogic
    {
        private readonly IAppliedKeywordService appliedKeywordService;
        private readonly IResponsesEndpoint responsesApi;
        private readonly IKeywordEndpoint keywordApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private AppliedKeywordsViewModel ViewModel { get; }
        private IList<IAppliedKeyword>? appliedKeywordsCache;

        public AppliedKeywordsPageLogic(
            IAppliedKeywordService appliedKeywordService, IResponsesEndpoint responsesApi, IKeywordEndpoint keywordApi,
            AppliedKeywordsViewModel dataContext)
        {
            this.appliedKeywordService = appliedKeywordService;
            this.responsesApi = responsesApi;
            this.keywordApi = keywordApi;
            ViewModel = dataContext;
            ViewModel.AppliedKeywords = new List<IAppliedKeyword>();
        }

        public void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox ??
                                throw new NullReferenceException(
                                    $"Expected '{nameof(sender)}' to not be null, but it was.");
            comboBoxSelection = (ArtificialIntelligenceType)comboBox.SelectedIndex;
            _ = UpdateAppliedKeywords();
        }

        internal async Task UpdateAppliedKeywords(bool forceUpdate = false)
        {
            if (forceUpdate || appliedKeywordsCache == null || (appliedKeywordsCache != null && !appliedKeywordsCache.Any()))
            {
                await UpdateAppliedKeywordsCache();
            }
            ViewModel.AppliedKeywords = appliedKeywordsCache!.Where(ak => ak.AiType == comboBoxSelection).ToList();
        }

        private async Task UpdateAppliedKeywordsCache()
        {
            var keywordsResponse = await keywordApi.GetAll(CancellationToken.None);
            EnsureSuccessStatusCode(keywordsResponse);

            var responsesResponse = await responsesApi.GetAll(CancellationToken.None);
            EnsureSuccessStatusCode(responsesResponse);

            appliedKeywordsCache = (await appliedKeywordService.CalculateAppliedKeywords(
                keywordsResponse.Content ?? throw new InvalidOperationException(),
                responsesResponse.Content ?? throw new InvalidOperationException())).ToList();
        }

        private void EnsureSuccessStatusCode<TEntity>(ApiResponse<List<TEntity>> response) where TEntity : class, IEntity
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }
    }
}
