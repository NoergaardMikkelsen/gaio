using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Uno.Endpoints;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Presentation.ViewModel;

namespace Statistics.Uno.Presentation;

public sealed partial class PromptsPage : Page
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        CREATED_AT = 1,
    }

    public PromptsPage()
    {
        var app = (App) Application.Current;

        IPromptEndpoint promptApi = app.Startup.ServiceProvider.GetService<IPromptEndpoint>() ??
                                    throw new NullReferenceException(
                                        $"Failed to acquire an instance implementing '{nameof(IPromptEndpoint)}'.");

        DataContext = new PromptsViewModel();

        var logic = new PromptsPageLogic(promptApi, (PromptsViewModel) DataContext);
        var ui = new PromptsPageUi(logic, (PromptsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdatePrompts();
    }

    private class PromptsPageUi : BasePageUi<PromptsPageLogic, PromptsViewModel>
    {
        public PromptsPageUi(PromptsPageLogic logic, PromptsViewModel dataContext) : base(logic, dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            DataGrid promptsDataGrid = DataGridFactory.CreateDataGrid(
                    DataContext, nameof(PromptsViewModel.Prompts), SetupDataGridColumns, SetupDataGridRowTemplate)
                .Grid(row: 1, column: 0, columnSpan: 5);

            grid.Children.Add(promptsDataGrid);
        }

        /// <inheritdoc />
        protected override void ConfigureGridRowsAndColumns(Grid grid)
        {
            const int rowOneHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridRowTemplate(dataGrid, Enum.GetValues<DataGridColumns>().Cast<int>(),
                x => x == (int)DataGridColumns.CREATED_AT, x => GetBindingPath((DataGridColumns)x));
        }

        private string GetBindingPath(DataGridColumns column)
        {
            return column switch
            {
                DataGridColumns.PROMPT_TEXT => nameof(Prompt.Text),
                DataGridColumns.CREATED_AT => nameof(Prompt.CreatedDateTime),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(dataGrid, Enum.GetValues<DataGridColumns>().Cast<int>(),
                x => GetBindingPath((DataGridColumns) x),
                x => x == (int)DataGridColumns.CREATED_AT, i => ((DataGridColumns) i ).ToString());
        }
    }

    private class PromptsPageLogic
    {
        private readonly IPromptEndpoint promptApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private PromptsViewModel DataContext { get; }

        public PromptsPageLogic(IPromptEndpoint promptApi, PromptsViewModel dataContext)
        {
            DataContext = dataContext;
            this.promptApi = promptApi;
        }

        internal async Task UpdatePrompts()
        {
            var apiResponse = await promptApi.GetAll(CancellationToken.None);

            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            var allPrompts = apiResponse.Content;

            if (allPrompts == null)
            {
                Console.WriteLine($"Failed to get all prompt entities.");
                return;
            }

            DataContext.Prompts = allPrompts;
        }
    }
}
