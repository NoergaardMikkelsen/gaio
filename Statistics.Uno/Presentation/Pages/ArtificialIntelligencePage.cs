using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

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
            StackPanel inputPanel = CreateInputPanel().Grid(row: 1, column: 0, columnSpan: 5);
            DataGrid aiDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(ArtificialIntelligenceViewModel.ArtificialIntelligences), SetupDataGridColumns)
                .Grid(row: 2, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel(() => Logic.UpdateArtificialIntelligences())
                .Grid(row: 3, column: 4);


            grid.Children.Add(buttonPanel);
            grid.Children.Add(inputPanel);
            grid.Children.Add(aiDataGrid);
            grid.Children.Add(refreshButtons);
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

    private class ArtificialIntelligencePageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private readonly Page page;
        private ArtificialIntelligenceViewModel ViewModel { get; }

        public ArtificialIntelligencePageLogic(
            IArtificialIntelligenceEndpoint aiApi, ArtificialIntelligenceViewModel viewModel, Page page)
        {
            this.aiApi = aiApi;
            this.page = page;
            ViewModel = viewModel;
        }

        internal async Task UpdateArtificialIntelligences()
        {
            ISearchableArtificialIntelligence searchable = BuildSearchableArtificialIntelligences();

            var apiResponse =
                await aiApi.GetAllByQuery(CancellationToken.None, (SearchableArtificialIntelligence) searchable);

            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                return;
            }

            var allAis = apiResponse.Content;

            if (allAis == null)
            {
                Console.WriteLine($"Failed to get all artificial intelligence entities.");
                return;
            }

            ViewModel.ArtificialIntelligences = allAis;
        }

        private ISearchableArtificialIntelligence BuildSearchableArtificialIntelligences()
        {
            return new SearchableArtificialIntelligence()
            {
                Key = ViewModel.SearchableAiKey ?? "", Name = ViewModel.SearchableAiName ?? "",
            };
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var aiId = (int) button.Tag;

            IArtificialIntelligence ai = ViewModel.ArtificialIntelligences.FirstOrDefault(x => x.Id == aiId) ??
                                         throw new NullReferenceException(
                                             $"Expected to find an artificial intelligence with id '{aiId}', but it was not found.");

            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromExisting(aiApi, ai, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
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
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateArtificialIntelligences();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromNew(aiApi, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateArtificialIntelligences();
        }

        public async void SearchFieldChanged(object? sender, string e)
        {
            if (e.Length < 5)
            {
                return;
            }

            await UpdateArtificialIntelligences();
        }
    }
}
