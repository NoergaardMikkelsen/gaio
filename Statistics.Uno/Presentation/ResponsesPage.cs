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

public sealed partial class ResponsesPage : Page
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        RESPONSE_TEXT = 1,
        RESPONSE_TIME = 2,
    }

    public ResponsesPage()
    {
        var app = (App) Application.Current;

        IArtificialIntelligenceEndpoint aiApi =
            app.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
            throw new NullReferenceException(
                $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");

        DataContext = new ResponsesViewModel();

        var logic = new ResponsesPageLogic(aiApi, (ResponsesViewModel) DataContext);
        var ui = new ResponsesPageUi(logic, (ResponsesViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class ResponsesPageUi
    {
        private readonly ResponsesPageLogic logic;
        private ResponsesViewModel DataContext { get; }

        public ResponsesPageUi(ResponsesPageLogic logic, ResponsesViewModel dataContext)
        {
            this.logic = logic;
            DataContext = dataContext;
        }

        public Grid CreateContentGrid()
        {
            var grid = new Grid();

            ConfigureGridRowsAndColumns(grid);

            DataGrid responsesDataGrid = CreateResponsesDataGrid().Grid(row: 1, column: 0, columnSpan: 5);
            ComboBox aiSelectionComboBox = CreateAiSelectionComboBox().Grid(row: 0, column: 4);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(responsesDataGrid);

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

        private DataGrid CreateResponsesDataGrid()
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

            dataGrid.SetBinding(DataGrid.ItemsSourceProperty,
                new Binding() {Path = nameof(ResponsesViewModel.Responses), Source = DataContext,});

            return dataGrid;
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            var stack = new StackPanel();
            var cells = Enum.GetValues<DataGridColumns>().Select(x =>
            {
                var block = new TextBlock() {Margin = new Thickness(10)};

                var binding = new Binding { Path = GetBindingPath(x), };
                if (x == DataGridColumns.RESPONSE_TIME)
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
                DataGridColumns.PROMPT_TEXT => $"{nameof(Response.Prompt)}.{nameof(Prompt.Text)}",
                DataGridColumns.RESPONSE_TEXT => $"{nameof(Response.Text)}",
                DataGridColumns.RESPONSE_TIME => $"{nameof(Response.CreatedDateTime)}",
                var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
            };
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            IList<DataGridColumn> columns = new List<DataGridColumn>();
            foreach (DataGridColumns value in Enum.GetValues<DataGridColumns>())
            {
                DataGridColumn column = value switch
                {
                    DataGridColumns.PROMPT_TEXT or DataGridColumns.RESPONSE_TEXT => CreateDataGridTextColumn(value),
                    DataGridColumns.RESPONSE_TIME => CreateDataGridTemplateColumn(value),
                    _ => throw new ArgumentOutOfRangeException()
                };

                column.Width = new DataGridLength(value == DataGridColumns.RESPONSE_TIME ? 1 : 2, DataGridLengthUnitType.Star);
                columns.Add(column);
            }

            dataGrid.Columns.AddRange(columns);
        }

        private DataGridTemplateColumn CreateDataGridTemplateColumn(DataGridColumns column)
        {
            var textBlock = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0),};

            var binding = new Binding {Path = GetBindingPath(column),};
            if (column == DataGridColumns.RESPONSE_TIME)
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

        private ComboBox CreateAiSelectionComboBox()
        {
            var comboBox = new ComboBox() {Margin = new Thickness(10),};

            var options = logic.EnumNamesToTitleCase(typeof(ArtificialIntelligenceType));

            comboBox.ItemsSource = options;
            comboBox.SelectionChanged += logic.ComboBoxOnSelectionChanged;
            comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;

            return comboBox;
        }
    }

    private class ResponsesPageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private ResponsesViewModel DataContext { get; }

        public ResponsesPageLogic(IArtificialIntelligenceEndpoint aiApi, ResponsesViewModel dataContext)
        {
            this.aiApi = aiApi;
            DataContext = dataContext;
            DataContext.Responses = new List<IResponse>();
        }

        internal IEnumerable<string> EnumNamesToTitleCase(Type enumType)
        {
            return Enum.GetNames(enumType).Select(x => x.ScreamingSnakeCaseToTitleCase());
        }

        public void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox ??
                                throw new NullReferenceException(
                                    $"Expected '{nameof(sender)}' to not be null, but it was.");
            comboBoxSelection = (ArtificialIntelligenceType) comboBox.SelectedIndex;

            UpdateResponses();
        }

        private async Task UpdateResponses()
        {
            var apiResponse = await aiApi.GetByQuery(CancellationToken.None,
                new SearchableArtificialIntelligence() {AiType = comboBoxSelection,});
            if (!apiResponse.IsSuccessful)
            {
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            ArtificialIntelligence? selectedAiEntity = apiResponse.Content;

            if (selectedAiEntity == null)
            {
                Console.WriteLine($"Failed to get selected Artificial Intelligence entity");
                return;
            }

            DataContext.Responses = selectedAiEntity.Responses.ToList();
        }
    }
}
