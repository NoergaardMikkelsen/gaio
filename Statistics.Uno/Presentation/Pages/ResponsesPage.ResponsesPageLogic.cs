using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage
{
    private class ResponsesPageLogic : BaseLogic<IResponse>
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
            updateCancellationTokenSource = new CancellationTokenSource();
        }

        internal override async Task UpdateDisplayedItems(bool forceUpdate = false)
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

                ViewModel.Responses = new ObservableCollection<IResponse>(responses);
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
            await UpdateDisplayedItems();
        }

        /// <inheritdoc />
        protected override string GetPropertyNameFromColumnHeader(string header)
        {
            var converter = new EnumToTitleCaseConverter();
            DataGridColumns column =
                (DataGridColumns) converter.ConvertBack(header, typeof(DataGridColumns), null, null);

            return column switch
            {
                DataGridColumns.RESPONSE_TEXT => nameof(IResponse.Text),
                DataGridColumns.PROMPT_TEXT => $"{nameof(IResponse.Prompt)}.{nameof(IPrompt.Text)}",
                DataGridColumns.CREATED_AT => nameof(IResponse.PromptId),
                _ => throw new ArgumentOutOfRangeException(nameof(header), $"Unexpected column header: {header}")
            };
        }

        /// <inheritdoc />
        protected override ObservableCollection<IResponse> GetCollection()
        {
            return ViewModel.Responses;
        }
    }
}
