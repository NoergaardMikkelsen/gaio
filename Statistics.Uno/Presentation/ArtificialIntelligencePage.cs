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

public sealed partial class ArtificialIntelligencePage : Page
{
    private enum DataGridColumns
    {
        NAME = 0,
        KEY = 1,
        AI_TYPE = 2,
        CREATED_AT = 3,
    }

    public ArtificialIntelligencePage()
    {
        var app = (App)Application.Current;

        IArtificialIntelligenceEndpoint aiApi = app.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
                                                throw new NullReferenceException(
                                                    $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");

        DataContext = new ArtificialIntelligenceViewModel();

        var logic = new ArtificialIntelligencePageLogic(aiApi, (ArtificialIntelligenceViewModel)DataContext);
        var ui = new ArtificialIntelligencePageUi(logic, (ArtificialIntelligenceViewModel)DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdateArtificialIntelligences();
    }

    private class ArtificialIntelligencePageUi : BasePageUi<ArtificialIntelligencePageLogic, ArtificialIntelligenceViewModel>
    {
        public ArtificialIntelligencePageUi(ArtificialIntelligencePageLogic logic, ArtificialIntelligenceViewModel dataContext) : base(logic, dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            DataGrid aiDataGrid = DataGridFactory.CreateDataGrid(
                    DataContext, nameof(ArtificialIntelligenceViewModel.ArtificialIntelligences), SetupDataGridColumns, SetupDataGridRowTemplate)
                .Grid(row: 1, column: 0, columnSpan: 5);

            grid.Children.Add(aiDataGrid);
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
                DataGridColumns.NAME => nameof(ArtificialIntelligence.Name),
                DataGridColumns.KEY => nameof(ArtificialIntelligence.Key),
                DataGridColumns.AI_TYPE => nameof(ArtificialIntelligence.AiType),
                DataGridColumns.CREATED_AT => nameof(ArtificialIntelligence.CreatedDateTime),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(dataGrid, Enum.GetValues<DataGridColumns>().Cast<int>(),
                x => GetBindingPath((DataGridColumns)x),
                x => x == (int)DataGridColumns.CREATED_AT, i => ((DataGridColumns)i).ToString());
        }
    }

    private class ArtificialIntelligencePageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceViewModel DataContext { get; }

        public ArtificialIntelligencePageLogic(IArtificialIntelligenceEndpoint aiApi, ArtificialIntelligenceViewModel dataContext)
        {
            this.aiApi = aiApi;
            DataContext = dataContext;
        }

        internal async Task UpdateArtificialIntelligences()
        {
            var apiResponse = await aiApi.GetAll(CancellationToken.None);

            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            var allAis = apiResponse.Content;

            if (allAis == null)
            {
                Console.WriteLine($"Failed to get all artificial intelligence entities.");
                return;
            }

            DataContext.ArtificialIntelligences = allAis;
        }
    }
}
