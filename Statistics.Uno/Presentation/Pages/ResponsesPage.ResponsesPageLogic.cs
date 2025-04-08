using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage
{
    private class ResponsesPageLogic
    {
        private readonly IResponseEndpoint responseApi;
        private readonly IActionEndpoint actionApi;
        private CancellationTokenSource updateCancellationTokenSource;
        private ResponsesViewModel ViewModel { get; }

        public ResponsesPageLogic(IResponseEndpoint responseApi, IActionEndpoint actionApi, ResponsesViewModel dataContext)
        {
            this.responseApi = responseApi;
            this.actionApi = actionApi;
            ViewModel = dataContext;
            ViewModel.Responses = new List<IResponse>();
            updateCancellationTokenSource = new CancellationTokenSource();
        }

        internal async Task UpdateResponses()
        {
            IComplexSearchable complexSearchable = BuildComplexSearchable();

            try
            {
                await updateCancellationTokenSource.CancelAsync();
                updateCancellationTokenSource = new CancellationTokenSource();
                Console.WriteLine($"Updating Responses...");
                ViewModel.UpdatingText = "Updating...";

                var apiResponse = await responseApi.GetAllByComplexQuery(updateCancellationTokenSource.Token,
                    (ComplexSearchable) complexSearchable);

                if (!apiResponse.IsSuccessful)
                {
                    Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                    return;
                }

                List<Response> responses = apiResponse.Content;

                if (responses == null)
                {
                    Console.WriteLine($"Failed to get selected Responses entities.");
                    return;
                }

                ViewModel.Responses = responses;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Update of Responses Cancelled...");
            }
            finally
            {
                ViewModel.UpdatingText = string.Empty;
            }
        }

        private IComplexSearchable BuildComplexSearchable()
        {
            return new ComplexSearchable()
            {
                SearchableResponse = new SearchableResponse() {Text = ViewModel.SearchableResponseText ?? "",},
                SearchableArtificialIntelligence = new SearchableArtificialIntelligence()
                {
                    AiType = ViewModel.SelectedAiType,
                },
                SearchablePrompt = new SearchablePrompt() {Text = ViewModel.SearchablePromptText ?? "",},
            };
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
            await UpdateResponses();
        }

        public async void SearchFieldChanged(object? sender, string e)
        {
            if (e.Length < 5)
            {
                return;
            }

            await UpdateResponses();
        }
    }
}
