using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.Core;

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

    private class PromptsPageLogic : BasePageLogic
    {
        private readonly IPromptEndpoint promptApi;
        private PromptsViewModel DataContext { get; }
        private readonly DispatcherQueue dispatchQueue;

        public PromptsPageLogic(IPromptEndpoint promptApi, PromptsViewModel dataContext)
        {
            DataContext = dataContext;
            this.promptApi = promptApi;
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

            DataContext.Prompts = allPrompts;
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

                var promptId = (int) button.Tag;
                Console.WriteLine($"Deleting prompt with id '{promptId}'...");

                await promptApi.DeleteById(CancellationToken.None, promptId);
            });
        }
    }
}
