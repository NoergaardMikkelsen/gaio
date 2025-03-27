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
        PROMPT_TIME = 1,
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

    private class PromptsPageUi
    {
        private readonly PromptsPageLogic logic;
        private PromptsViewModel DataContext { get; }

        public PromptsPageUi(PromptsPageLogic logic, PromptsViewModel dataContext)
        {
            DataContext = dataContext;
            this.logic = logic;
        }

        public Grid CreateContentGrid()
        {
            var grid = new Grid();

            ConfigureGridRowsAndColumns(grid);

            DataGrid promptsDataGrid = CreatePromptsDataGrid().Grid(row: 1, column: 0, columnSpan: 5);

            grid.Children.Add(promptsDataGrid);

            return grid;
        }

        private void ConfigureGridRowsAndColumns(Grid grid)
        {
            const int rowOneHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star));
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 5).ToArray());
        }

        private DataGrid CreatePromptsDataGrid()
        {
            var dataGrid = new DataGrid()
            {
                CanUserReorderColumns = false, CanUserResizeColumns = true, CanUserSortColumns = true,
                SelectionMode = DataGridSelectionMode.Single,
                ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader, AutoGenerateColumns = false,
                Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            SetupDataGridColumns(dataGrid);
            SetupDataGridRowTemplate(dataGrid);

            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() {Path = nameof(PromptsViewModel.Prompts), Source = DataContext});

            return dataGrid;
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            var stack = new StackPanel();
            var cells = Enum.GetValues<DataGridColumns>().Select(x =>
            {
                var block = new TextBlock() {Margin = new Thickness(10)};

                var binding = new Binding {Path = GetBindingPath(x),};
                if (x == DataGridColumns.PROMPT_TIME)
                {
                    binding.Converter = new StringFormatConverter();
                    binding.ConverterParameter = "{0:dd/MM/yyyy HH:mm:ss}";
                }

                block.SetBinding(TextBlock.TextProperty, binding);

                return block;
            });
            stack.Children.AddRange(cells);
            dataGrid.RowDetailsTemplate = new DataTemplate(() => stack);
        }

        private string GetBindingPath(DataGridColumns column)
        {
            return column switch
            {
                DataGridColumns.PROMPT_TEXT => nameof(Prompt.Text),
                DataGridColumns.PROMPT_TIME => nameof(Prompt.CreatedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            IList<DataGridColumn> columns = new List<DataGridColumn>();
            foreach (DataGridColumns value in Enum.GetValues<DataGridColumns>())
            {
                DataGridColumn column = value switch
                {
                    DataGridColumns.PROMPT_TEXT => CreateDataGridTextColumn(value),
                    DataGridColumns.PROMPT_TIME => CreateDataGridTemplateColumn(value),
                    _ => throw new ArgumentOutOfRangeException()
                };

                column.Width = new DataGridLength(value == DataGridColumns.PROMPT_TIME ? 1 : 2,
                    DataGridLengthUnitType.Star);
                columns.Add(column);
            }

            dataGrid.Columns.AddRange(columns);
        }

        private DataGridTemplateColumn CreateDataGridTemplateColumn(DataGridColumns column)
        {
            var textBlock = new TextBlock()
                {VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0),};

            var binding = new Binding {Path = GetBindingPath(column),};
            if (column == DataGridColumns.PROMPT_TIME)
            {
                binding.Converter = new StringFormatConverter();
                binding.ConverterParameter = "{0:dd/MM/yyyy HH:mm:ss}";
            }

            textBlock.SetBinding(TextBlock.TextProperty, binding);

            return new DataGridTemplateColumn()
            {
                Header = column.ToString().ScreamingSnakeCaseToTitleCase(),
                Tag = column,
                CellTemplate = new DataTemplate(() => textBlock),
            };
        }

        private DataGridTextColumn CreateDataGridTextColumn(DataGridColumns column)
        {
            return new DataGridTextColumn()
            {
                Header = column.ToString().ScreamingSnakeCaseToTitleCase(),
                Tag = column,
                Binding = new Binding
                {
                    Path = GetBindingPath(column),
                },
            };
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
