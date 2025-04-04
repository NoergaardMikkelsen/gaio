using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class PromptsPage : BasePage
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        CREATED_AT = 1,
        LAST_UPDATED_AT = 2,
        ACTIONS = 3,
    }

    public PromptsPage()
    {
        IPromptEndpoint promptApi = GetPromptApi();
        IActionEndpoint actionApi = GetActionApi();

        DataContext = new PromptsViewModel();

        var logic = new PromptsPageLogic(promptApi, actionApi, (PromptsViewModel) DataContext, this);
        var ui = new PromptsPageUi(logic, (PromptsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdatePrompts();
    }

    private class PromptsPageUi : BaseUi<PromptsPageLogic, PromptsViewModel>
    {
        public PromptsPageUi(PromptsPageLogic logic, PromptsViewModel dataContext) : base(logic, dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            StackPanel buttonPanel = CreateButtonsPanel().Grid(row: 0, column: 0, columnSpan: 2);
            DataGrid promptsDataGrid = DataGridFactory
                .CreateDataGrid(ViewModel, nameof(PromptsViewModel.Prompts), SetupDataGridColumns)
                .Grid(row: 1, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel(() => Logic.UpdatePrompts()).Grid(row: 2, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(promptsDataGrid);
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

            var executeAllPromptsButton = new Button()
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
            const int rowOneHeight = 8;
            const int rowThreeHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight - rowThreeHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star), new GridLength(rowThreeHeight, GridUnitType.Star));
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
            var stackPanel = new StackPanel() {Orientation = Orientation.Horizontal,};

            var editButton = new Button()
            {
                Content = "Edit",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            editButton.Click += Logic.EditButtonOnClick;
            editButton.Tag(x => x.Binding(nameof(Prompt.Id)));
            var deleteButton = new Button()
            {
                Content = "Delete",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            deleteButton.Click += Logic.DeleteButtonOnClick;
            deleteButton.Tag(x => x.Binding(nameof(Prompt.Id)));

            var executeButton = new Button()
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

    private class PromptsPageLogic
    {
        private readonly IPromptEndpoint promptApi;
        private readonly IActionEndpoint actionApi;
        private readonly Page page;
        private PromptsViewModel ViewModel { get; }

        public PromptsPageLogic(
            IPromptEndpoint promptApi, IActionEndpoint actionApi, PromptsViewModel viewModel, PromptsPage page)
        {
            ViewModel = viewModel;
            this.promptApi = promptApi;
            this.actionApi = actionApi;
            this.page = page;
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

            ViewModel.Prompts = allPrompts;
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var promptId = (int) button.Tag;

            IPrompt prompt = ViewModel.Prompts.FirstOrDefault(x => x.Id == promptId) ??
                             throw new NullReferenceException(
                                 $"Expected to find a prompt with id '{promptId}', but it was not found.");

            await ContentDialogFactory.ShowBuildPromptDialogFromExisting(promptApi, prompt, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdatePrompts();
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

            var promptId = (int) button.Tag;

            await promptApi.DeleteById(CancellationToken.None, promptId);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdatePrompts();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildPromptDialogFromNew(promptApi, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await UpdatePrompts();
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
        }

        public async void ExecutePromptOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var promptId = (int) button.Tag;

            button.IsEnabled = false;
            await actionApi.ExecutePromptById(CancellationToken.None, promptId);
            button.IsEnabled = true;
        }
    }
}
