using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class PromptsPage : Page
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        CREATED_AT = 1,
        ACTIONS = 2,
    }

    public PromptsPage()
    {
        var app = (App) Application.Current;

        IPromptEndpoint promptApi = app.Startup.ServiceProvider.GetService<IPromptEndpoint>() ??
                                    throw new NullReferenceException(
                                        $"Failed to acquire an instance implementing '{nameof(IPromptEndpoint)}'.");

        DataContext = new PromptsViewModel();

        var logic = new PromptsPageLogic(promptApi, (PromptsViewModel) DataContext, this);
        var ui = new PromptsPageUi(logic, (PromptsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdatePrompts();
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
            DataGrid promptsDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(PromptsViewModel.Prompts), SetupDataGridColumns, SetupDataGridRowTemplate)
                .Grid(row: 1, column: 0, columnSpan: 5);
            Button refreshButton = CreateRefreshButton().Grid(row: 2, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(promptsDataGrid);
            grid.Children.Add(refreshButton);
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

        private Button CreateRefreshButton()
        {
            var button = new Button()
            {
                Content = "Refresh",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
            };

            button.Click += async (_, _) => await Logic.UpdatePrompts();
            return button;
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
                Enum.GetValues<DataGridColumns>().Cast<int>(), x => x == (int) DataGridColumns.CREATED_AT,
                GetColumnBindingPath, BuildActionsElement));
        }

        private string GetColumnBindingPath(int columnNumber)
        {
            var column = (DataGridColumns) columnNumber;

            return column switch
            {
                DataGridColumns.PROMPT_TEXT => nameof(Prompt.Text),
                DataGridColumns.CREATED_AT => nameof(Prompt.CreatedDateTime),
                DataGridColumns.ACTIONS => nameof(Prompt.Id),
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
                DataGridColumns.PROMPT_TEXT => 100,
                DataGridColumns.CREATED_AT => 25,
                DataGridColumns.ACTIONS => 25,
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

            stackPanel.Children.Add(editButton);
            stackPanel.Children.Add(deleteButton);

            return stackPanel;
        }
    }

    private class PromptsPageLogic
    {
        private readonly IPromptEndpoint promptApi;
        private readonly Page page;
        private PromptsViewModel ViewModel { get; }
        private readonly DispatcherQueue dispatchQueue;

        public PromptsPageLogic(IPromptEndpoint promptApi, PromptsViewModel dataContext, Page page)
        {
            ViewModel = dataContext;
            this.promptApi = promptApi;
            this.page = page;
            dispatchQueue = DispatcherQueue.GetForCurrentThread();
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

            var prompt = ViewModel.Prompts.FirstOrDefault(x => x.Id == promptId) ??
                         throw new NullReferenceException(
                             $"Expected to find a prompt with id '{promptId}', but it was not found.");

            await ContentDialogFactory.ShowBuildPromptDialogFromExisting(promptApi, prompt, page.XamlRoot);
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
            await UpdatePrompts();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildPromptDialogFromNew(promptApi, page.XamlRoot);
            await UpdatePrompts();
        }
    }
}
