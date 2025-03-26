using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Uno.Endpoints;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
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

        IResponsesEndpoint responseApi = app.Startup.ServiceProvider.GetService<IResponsesEndpoint>() ??
                                         throw new NullReferenceException(
                                             $"Failed to acquire an instance implementing '{nameof(IResponsesEndpoint)}'.");
        IArtificialIntelligenceEndpoint aiApi =
            app.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
            throw new NullReferenceException(
                $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");

        DataContext = new ResponsesViewModel();

        var logic = new ResponsesPageLogic(responseApi, aiApi, (ResponsesViewModel) DataContext);
        var ui = new ResponsesPageUi(logic, (ResponsesViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class ResponsesPageUi
    {
        private readonly ResponsesPageLogic logic;
        private ResponsesViewModel DataContext { get; init; }

        public ResponsesPageUi(ResponsesPageLogic logic, ResponsesViewModel dataContext)
        {
            this.logic = logic;
            DataContext = dataContext;
        }

        public Grid CreateContentGrid()
        {
            var grid = new Grid();

            const int rowOneHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star));

            DataGrid responsesItemsView = CreateResponsesDataGrid().Grid(row: 1, column: 0);
            ComboBox aiSelectionComboBox = CreateAiSelectionComboBox().Grid(row: 0, column: 0);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(responsesItemsView);

            return grid;
        }

        private DataGrid CreateResponsesDataGrid()
        {
            var dataGrid = new DataGrid()
            {
                CanUserReorderColumns = false, CanUserResizeColumns = true, CanUserSortColumns = true,
                SelectionMode = DataGridSelectionMode.Single,
                ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader, AutoGenerateColumns = false,
            };

            logic.RegisterResponsesDataGrid(dataGrid);

            //dataGrid.ItemsSource(x => x.Binding(() => logic.Responses));

            SetupDataGridColumns(dataGrid);
            SetupDataGridRowTemplate(dataGrid);

            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() {Path = nameof(ResponsesViewModel.Responses), Source = DataContext});

            return dataGrid;
        }

        private void SetupDataGridRowTemplate(DataGrid dataGrid)
        {
            var stack = new StackPanel();
            var cells = Enum.GetValues<DataGridColumns>().Select(x =>
            {
                var block = new TextBlock();

                switch (x)
                {
                    case DataGridColumns.PROMPT_TEXT:
                        block.Text(x => x.Binding($"{nameof(Response.Prompt)}.{nameof(Prompt.Text)}"));
                        break;
                    case DataGridColumns.RESPONSE_TEXT:
                        block.Text(x => x.Binding($"{nameof(Response.Text)}"));
                        break;
                    case DataGridColumns.RESPONSE_TIME:
                        block.Text(x => x.Binding($"{nameof(Response.CreatedDateTime)}"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }

                return block;
            });
            stack.Children.AddRange(cells);
            dataGrid.RowDetailsTemplate = new DataTemplate(() => stack);
        }

        private void SetupDataGridColumns(DataGrid dataGrid)
        {
            var columns = Enum.GetValues<DataGridColumns>().Select(x =>
            {
                var column = new DataGridTextColumn()
                    {Header = logic.EnumStringValueToTitleCase(x.ToString()), Tag = x, Binding = new Binding()};

                switch (x)
                {
                    case DataGridColumns.PROMPT_TEXT:
                        column.Binding.Path = new PropertyPath($"{nameof(Response.Prompt)}.{nameof(Prompt.Text)}");
                        break;
                    case DataGridColumns.RESPONSE_TEXT:
                        column.Binding.Path = new PropertyPath($"{nameof(Response.Text)}");
                        break;
                    case DataGridColumns.RESPONSE_TIME:
                        column.Binding.Path = new PropertyPath($"{nameof(Response.CreatedDateTime)}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }

                return column;
            });

            dataGrid.Columns.AddRange(columns);
        }

        private ComboBox CreateAiSelectionComboBox()
        {
            var comboBox = new ComboBox();

            var options = logic.EnumNamesToTitleCase(typeof(ArtificialIntelligenceType));

            comboBox.ItemsSource = options;
            comboBox.SelectionChanged += logic.ComboBoxOnSelectionChanged;
            comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;

            return comboBox;
        }
    }

    private partial class ResponsesPageLogic
    {
        private readonly IResponsesEndpoint responseApi;
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private DataGrid dataGrid;
        private readonly DispatcherQueue dispatchQueue;
        private ResponsesViewModel DataContext { get; init; }

        public ResponsesPageLogic(
            IResponsesEndpoint responseApi, IArtificialIntelligenceEndpoint aiApi, ResponsesViewModel dataContext)
        {
            this.responseApi = responseApi;
            this.aiApi = aiApi;
            DataContext = dataContext;
            DataContext.Responses = new List<IResponse>();
            dispatchQueue = DispatcherQueue.GetForCurrentThread();
        }

        internal IEnumerable<string> EnumNamesToTitleCase(Type enumType)
        {
            return Enum.GetNames(enumType).Select(EnumStringValueToTitleCase);
        }

        internal string EnumStringValueToTitleCase(string enumString)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(string.Join(' ', enumString.Split('_')).ToLower());
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
            Console.WriteLine($"Requesting Ai with type: '{comboBoxSelection}'");
            var apiResponse = await aiApi.GetByQuery(CancellationToken.None,
                new SearchableArtificialIntelligence() {AiType = comboBoxSelection,});
            if (!apiResponse.IsSuccessful)
            {
                // System.InvalidOperationException: Each parameter in the deserialization constructor on type 'Statistics.Shared.Models.Entity.ArtificialIntelligence'
                // must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object.
                // Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled. The match can be case-insensitive.
                Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
            }

            ArtificialIntelligence? selectedAiEntity = apiResponse.Content;

            if (selectedAiEntity == null)
            {
                Console.WriteLine($"Failed to get selected Artificial Intelligence entity");
                return;
            }

            Console.WriteLine($"Received Entity: {JsonConvert.SerializeObject(selectedAiEntity)}");


            dispatchQueue.TryEnqueue(() =>
            {
                Console.WriteLine($"Updating Responses... - {JsonConvert.SerializeObject(selectedAiEntity)}");
                DataContext.Responses = selectedAiEntity.Responses.ToList();
            });

            //Responses = selectedAiEntity.Responses.ToList();
        }

        public void RegisterResponsesDataGrid(DataGrid dataGridReference)
        {
            dataGrid = dataGridReference;
        }
    }
}
