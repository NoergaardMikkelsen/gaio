using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core.Converters;
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

        public void SortItems(object? sender, DataGridColumnEventArgs e)
        {
            if (sender is not DataGrid dataGrid || e.Column == null)
                return;

            var propertyName = e.Column != null
                ? GetPropertyNameFromColumnHeader(e.Column.Header.ToString() ?? throw new InvalidOperationException())
                : throw new ArgumentNullException(nameof(e.Column));

            // Determine the sort direction
            var sortDirection =
                e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending
                    ? DataGridSortDirection.Ascending
                    : DataGridSortDirection.Descending;

            dataGrid.Columns.ForEach(column => column.SortDirection = null);

            // Update the column's sort direction
            e.Column.SortDirection = sortDirection;

            // Perform the sorting
            var sortedItems = sortDirection == DataGridSortDirection.Ascending
                ? ViewModel.Responses.OrderBy(item => GetPropertyValue(item, propertyName)).ToList()
                : ViewModel.Responses.OrderByDescending(item => GetPropertyValue(item, propertyName)).ToList();

            // Update the ObservableCollection
            ViewModel.Responses.Clear();
            foreach (var item in sortedItems)
            {
                ViewModel.Responses.Add(item);
            }
        }

        private string GetPropertyNameFromColumnHeader(string header)
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

        private object? GetPropertyValue(object obj, string propertyName)
        {
            if (!propertyName.Contains('.'))
            {
                // Handle direct property
                return obj.GetSortableValue(propertyName);
            }

            // Handle child object property
            var parts = propertyName.Split('.');
            var parentProperty = obj.GetType().GetProperty(parts[0]);
            var childObject = parentProperty?.GetValue(obj);
            return childObject?.GetSortableValue(parts[1]);

        }


    }
}
