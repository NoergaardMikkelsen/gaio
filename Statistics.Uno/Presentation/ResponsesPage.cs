using Statistics.Shared.Abstraction.Enum;
using Statistics.Uno.Endpoints;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Uno.Presentation;

public sealed partial class ResponsesPage : Page
{
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

        var logic = new ResponsesPageLogic(responseApi, aiApi);
        var ui = new ResponsesPageUi(logic);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class ResponsesPageUi
    {
        private readonly ResponsesPageLogic logic;

        public ResponsesPageUi(ResponsesPageLogic logic)
        {
            this.logic = logic;
        }

        public Grid CreateContentGrid()
        {
            var grid = new Grid();

            const int rowOneHeight = 8;
            const int rowTwoHeight = 100 - rowOneHeight;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(new GridLength(rowOneHeight, GridUnitType.Star),
                new GridLength(rowTwoHeight, GridUnitType.Star));

            ItemsView responsesItemsView = CreateResponsesItemsView().Grid(row: 1, column: 0);
            ComboBox aiSelectionComboBox = CreateAiSelectionComboBox().Grid(row: 0, column: 0);

            grid.Children.Add(aiSelectionComboBox);
            grid.Children.Add(responsesItemsView);

            return grid;
        }

        private ItemsView CreateResponsesItemsView()
        {
            var itemsView = new ItemsView();

            logic.RegisterResponsesItemsView(itemsView);

            itemsView.ItemsSource(x => x.Binding(() => logic.Responses));

            return itemsView;
        }

        private ComboBox CreateAiSelectionComboBox()
        {
            var comboBox = new ComboBox();

            var options = Enum.GetNames(typeof(ArtificialIntelligenceType)).Select(x => x.Split('_')).Select(x =>
                Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(string.Join(' ', x).ToLower()));

            comboBox.ItemsSource = options;
            comboBox.SelectionChanged += logic.ComboBoxOnSelectionChanged;
            comboBox.SelectedIndex = (int) ArtificialIntelligenceType.OPEN_AI;

            return comboBox;
        }
    }

    private class ResponsesPageLogic
    {
        private readonly IResponsesEndpoint responseApi;
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private ArtificialIntelligenceType comboBoxSelection;
        private ItemsView itemsView;
        private readonly DispatcherQueue dispatchQueue;

        internal IList<IResponse> Responses { get; private set; }

        public ResponsesPageLogic(IResponsesEndpoint responseApi, IArtificialIntelligenceEndpoint aiApi)
        {
            this.responseApi = responseApi;
            this.aiApi = aiApi;
            Responses = new List<IResponse>();
            dispatchQueue = DispatcherQueue.GetForCurrentThread();
        }

        public void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox ??
                                throw new NullReferenceException(
                                    $"Expected '{nameof(sender)}' to not be null, but it was.");
            comboBoxSelection = (ArtificialIntelligenceType) comboBox.SelectedIndex;

            UpdateResponsesItemsView();
        }

        private async Task UpdateResponsesItemsView()
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
                Responses = selectedAiEntity.Responses.ToList();
            });

            //Responses = selectedAiEntity.Responses.ToList();
        }

        public void RegisterResponsesItemsView(ItemsView itemsViewReference)
        {
            itemsView = itemsViewReference;
        }
    }
}
