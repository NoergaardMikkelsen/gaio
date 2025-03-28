using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.Core;
using Statistics.Uno.Presentation.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ArtificialIntelligencePage : Page
{
    private enum DataGridColumns
    {
        NAME = 0,
        KEY = 1,
        AI_TYPE = 2,
        CREATED_AT = 3,
        ACTIONS = 4,
    }

    public ArtificialIntelligencePage()
    {
        var app = (App) Application.Current;

        IArtificialIntelligenceEndpoint aiApi =
            app.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
            throw new NullReferenceException(
                $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");

        DataContext = new ArtificialIntelligenceViewModel();

        var logic = new ArtificialIntelligencePageLogic(aiApi, (ArtificialIntelligenceViewModel) DataContext);
        var ui = new ArtificialIntelligencePageUi(logic, (ArtificialIntelligenceViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdateArtificialIntelligences();
    }

    private class
        ArtificialIntelligencePageUi : BasePageUi<ArtificialIntelligencePageLogic, ArtificialIntelligenceViewModel>
    {
        public ArtificialIntelligencePageUi(
            ArtificialIntelligencePageLogic logic, ArtificialIntelligenceViewModel dataContext) : base(logic,
            dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            DataGrid aiDataGrid = DataGridFactory.CreateDataGrid(
                DataContext, nameof(ArtificialIntelligenceViewModel.ArtificialIntelligences), SetupDataGridColumns,
                SetupDataGridRowTemplate).Grid(row: 1, column: 0, columnSpan: 5);

            grid.Children.Add(aiDataGrid);
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
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
            DataGridFactory.SetupDataGridRowTemplate(new SetupRowArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), x => x == (int) DataGridColumns.CREATED_AT,
                GetColumnBindingPath, BuildActionsElement));
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
                DataGridColumns.ACTIONS => nameof(ArtificialIntelligence.Id),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            DataGridFactory.SetupDataGridColumns(new SetupColumnsArguments(dataGrid,
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetColumnBindingPath,
                x => x == (int) DataGridColumns.CREATED_AT, i => ((DataGridColumns) i).ToString(), GetColumnStarWidth,
                BuildActionsElement));
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
                DataGridColumns.ACTIONS => 35,
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private FrameworkElement BuildActionsElement(int columnEnumAsInt)
        {
            var stackPanel = new StackPanel() {Orientation = Orientation.Horizontal,};

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

    private class ArtificialIntelligencePageLogic : BasePageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceViewModel DataContext { get; }
        private readonly DispatcherQueue dispatchQueue;

        public ArtificialIntelligencePageLogic(
            IArtificialIntelligenceEndpoint aiApi, ArtificialIntelligenceViewModel dataContext)
        {
            this.aiApi = aiApi;
            DataContext = dataContext;
            dispatchQueue = DispatcherQueue.GetForCurrentThread();
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

        public void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Edit button clicked.");
        }

        public void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Delete button clicked.");
            dispatchQueue.EnqueueAsync(async () =>
            {
                Console.WriteLine("Delete button clicked in dispatcher queue.");
                ContentDialogResult result = await ShowConfirmationDialog("Delete Confirmation",
                    "Are you sure you want to delete this item?");

                if (result != ContentDialogResult.Primary)
                {
                    Console.WriteLine("User did not confirm deletion.");
                    return;
                }

                Button button = sender as Button ??
                                throw new NullReferenceException(
                                    $"Expected '{nameof(sender)}' to not be null, but it was.");

                var aiId = (int) button.Tag;
                Console.WriteLine($"Deleting prompt with id '{aiId}'...");

                await aiApi.DeleteById(CancellationToken.None, aiId);
            });
        }
    }
}
