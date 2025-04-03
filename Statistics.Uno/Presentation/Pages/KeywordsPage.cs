using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class KeywordsPage : BasePage
{
    private enum DataGridColumns
    {
        KEYWORD_TEXT = 0,
        USE_REGEX = 1,
        START_SEARCH = 2,
        END_SEARCH = 3,
        CREATED_AT = 4,
        LAST_UPDATED_AT = 5,
        ACTIONS = 6,
    }

    public KeywordsPage()
    {
        IKeywordEndpoint keywordApi = GetKeywordApi();

        DataContext = new KeywordsViewModel();

        var logic = new KeywordsPageLogic(keywordApi, (KeywordsViewModel) DataContext, this);
        var ui = new KeywordsPageUi(logic, (KeywordsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateKeywords();
    }

    private class KeywordsPageUi : BaseUi<KeywordsPageLogic, KeywordsViewModel>
    {
        public KeywordsPageUi(KeywordsPageLogic logic, KeywordsViewModel dataContext) : base(logic, dataContext)
        {
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            StackPanel buttonPanel = CreateButtonsPanel().Grid(row: 0, column: 0, columnSpan: 2);
            DataGrid keywordsDataGrid = DataGridFactory.CreateDataGrid(
                    ViewModel, nameof(KeywordsViewModel.Keywords), SetupDataGridColumns, SetupDataGridRowTemplate)
                .Grid(row: 1, column: 0, columnSpan: 5);
            StackPanel refreshButtons = CreateRefreshButtonsPanel(() => Logic.UpdateKeywords()).Grid(row: 2, column: 4);

            grid.Children.Add(buttonPanel);
            grid.Children.Add(keywordsDataGrid);
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
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.KEYWORD_TEXT => nameof(Keyword.Text),
                DataGridColumns.CREATED_AT => nameof(Keyword.CreatedDateTime),
                DataGridColumns.LAST_UPDATED_AT => nameof(Keyword.UpdatedDateTime),
                DataGridColumns.START_SEARCH => nameof(Keyword.StartSearch),
                DataGridColumns.END_SEARCH => nameof(Keyword.EndSearch),
                DataGridColumns.USE_REGEX => nameof(Keyword.UseRegex),
                DataGridColumns.ACTIONS => nameof(Keyword.Id),
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private IValueConverter? GetValueConverterForColumn(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.CREATED_AT => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.LAST_UPDATED_AT => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.START_SEARCH => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.END_SEARCH => new UtcDateTimeToLocalStringConverter(),
                DataGridColumns.USE_REGEX => new BooleanToYesNoConverter(),
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
            return ((DataGridColumns)columnNumber).ToString();
        }

        private int GetColumnStarWidth(int columnNumber)
        {
            var column = (DataGridColumns)columnNumber;

            return column switch
            {
                DataGridColumns.KEYWORD_TEXT => 100,
                DataGridColumns.CREATED_AT => 25,
                DataGridColumns.LAST_UPDATED_AT => 25,
                DataGridColumns.START_SEARCH => 25,
                DataGridColumns.END_SEARCH => 25,
                DataGridColumns.USE_REGEX => 15,
                DataGridColumns.ACTIONS => 25,
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private FrameworkElement BuildActionsElement(int columnEnumAsInt)
        {
            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, };

            var editButton = new Button()
            {
                Content = "Edit",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            editButton.Click += Logic.EditButtonOnClick;
            editButton.Tag(x => x.Binding(nameof(Keyword.Id)));
            var deleteButton = new Button()
            {
                Content = "Delete",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            deleteButton.Click += Logic.DeleteButtonOnClick;
            deleteButton.Tag(x => x.Binding(nameof(Keyword.Id)));

            stackPanel.Children.Add(editButton);
            stackPanel.Children.Add(deleteButton);

            return stackPanel;
        }
    }

    private class KeywordsPageLogic
    {
        private readonly IKeywordEndpoint keywordApi;
        private readonly Page page;
        private KeywordsViewModel ViewModel { get; }

        public KeywordsPageLogic(IKeywordEndpoint keywordApi, KeywordsViewModel dataContext, Page page)
        {
            ViewModel = dataContext;
            this.keywordApi = keywordApi;
            this.page = page;
        }

        internal async Task UpdateKeywords()
        {
            var apiResponse = await keywordApi.GetAll(CancellationToken.None);

            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            var allKeywords = apiResponse.Content;

            if (allKeywords == null)
            {
                Console.WriteLine($"Failed to get all keyword entities.");
                return;
            }

            ViewModel.Keywords = allKeywords;
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var keywordId = (int)button.Tag;

            IKeyword keyword = ViewModel.Keywords.FirstOrDefault(x => x.Id == keywordId) ??
                               throw new NullReferenceException(
                                   $"Expected to find a keyword with id '{keywordId}', but it was not found.");

            await ContentDialogFactory.ShowBuildKeywordDialogFromExisting(keywordApi, keyword, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateKeywords();
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

            var keywordId = (int)button.Tag;

            await keywordApi.DeleteById(CancellationToken.None, keywordId);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateKeywords();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildKeywordDialogFromNew(keywordApi, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await UpdateKeywords();
        }
    }
}


