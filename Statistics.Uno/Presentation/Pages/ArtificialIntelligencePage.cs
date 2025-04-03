using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;
using ArtificialIntelligenceViewModel = Statistics.Uno.Presentation.Pages.ViewModel.ArtificialIntelligenceViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ArtificialIntelligencePage : BasePage
{
    private enum DataGridColumns
    {
        NAME = 0,
        KEY = 1,
        AI_TYPE = 2,
        CREATED_AT = 3,
        LAST_UPDATED_AT = 4,
        ACTIONS = 5,
    }

    public ArtificialIntelligencePage()
    {
        IArtificialIntelligenceEndpoint aiApi = GetAiApi();

        DataContext = new ArtificialIntelligenceViewModel();

        var logic = new ArtificialIntelligencePageLogic(aiApi, (ArtificialIntelligenceViewModel) DataContext, this);
        var ui = new ArtificialIntelligencePageUi(logic, (ArtificialIntelligenceViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateArtificialIntelligences();
    }

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
            DataGrid aiDataGrid = DataGridFactory.CreateDataGrid(
                ViewModel, nameof(ArtificialIntelligenceViewModel.ArtificialIntelligences), SetupDataGridColumns,
                SetupDataGridRowTemplate).Grid(row: 1, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel(() => Logic.UpdateArtificialIntelligences())
                .Grid(row: 2, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(aiDataGrid);
            grid.Children.Add(refreshButtons);
        }

        private StackPanel CreateButtonsPanel()
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10),
            };

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
                Enum.GetValues<DataGridColumns>().Cast<int>(), GetValueConverterForColumn, GetColumnBindingPath,
                BuildActionsElement));
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

    private class ArtificialIntelligencePageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private readonly Page page;
        private ArtificialIntelligenceViewModel DataContext { get; }

        public ArtificialIntelligencePageLogic(
            IArtificialIntelligenceEndpoint aiApi, ArtificialIntelligenceViewModel dataContext, Page page)
        {
            this.aiApi = aiApi;
            this.page = page;
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

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var aiId = (int) button.Tag;

            IArtificialIntelligence ai = DataContext.ArtificialIntelligences.FirstOrDefault(x => x.Id == aiId) ??
                                         throw new NullReferenceException(
                                             $"Expected to find an artificial intelligence with id '{aiId}', but it was not found.");

            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromExisting(aiApi, ai, page.XamlRoot);
            await UpdateArtificialIntelligences();
        }

        public async void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await ContentDialogFactory.ShowConfirmationDialog("Delete Confirmation",
                "Are you sure you want to delete this item?", page.XamlRoot);

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var aiId = (int) button.Tag;

            await aiApi.DeleteById(CancellationToken.None, aiId);
            await UpdateArtificialIntelligences();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromNew(aiApi, page.XamlRoot);
            await UpdateArtificialIntelligences();
        }
    }
}
