using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ArtificialIntelligencePage
{
    private class ArtificialIntelligencePageLogic
    {
        private readonly IArtificialIntelligenceEndpoint aiApi;
        private readonly Page page;
        private CancellationTokenSource updateCancellationTokenSource;
        private ArtificialIntelligenceViewModel ViewModel { get; }

        public ArtificialIntelligencePageLogic(
            IArtificialIntelligenceEndpoint aiApi, ArtificialIntelligenceViewModel viewModel, Page page)
        {
            this.aiApi = aiApi;
            this.page = page;
            ViewModel = viewModel;
            updateCancellationTokenSource = new CancellationTokenSource();
        }

        internal async Task UpdateArtificialIntelligences()
        {
            ISearchableArtificialIntelligence searchable = BuildSearchableArtificialIntelligences();

            try
            {
                await updateCancellationTokenSource.CancelAsync();
                updateCancellationTokenSource = new CancellationTokenSource();

                Console.WriteLine($"Updating Artificial Intelligences...");
                ViewModel.UpdatingText = "Updating...";

                var apiResponse = await aiApi.GetAllByQuery(updateCancellationTokenSource.Token,
                    (SearchableArtificialIntelligence) searchable);

                if (!apiResponse.IsSuccessful)
                {
                    Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                    return;
                }

                var allAis = apiResponse.Content;

                if (allAis == null)
                {
                    Console.WriteLine($"Failed to get all artificial intelligence entities.");
                    return;
                }

                ViewModel.ArtificialIntelligences = new ObservableCollection<IArtificialIntelligence>(allAis);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Update of Artificial Intelligences Cancelled...");
            }
            finally
            {
                ViewModel.UpdatingText = string.Empty;
            }
        }

        private ISearchableArtificialIntelligence BuildSearchableArtificialIntelligences()
        {
            return new SearchableArtificialIntelligence()
            {
                Key = ViewModel.SearchableAiKey ?? "",
                Name = ViewModel.SearchableAiName ?? "",
            };
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var aiId = (int) button.Tag;

            IArtificialIntelligence ai = ViewModel.ArtificialIntelligences.FirstOrDefault(x => x.Id == aiId) ??
                                         throw new NullReferenceException(
                                             $"Expected to find an artificial intelligence with id '{aiId}', but it was not found.");

            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromExisting(aiApi, ai, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateArtificialIntelligences();
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

            var aiId = (int) button.Tag;

            await aiApi.DeleteById(CancellationToken.None, aiId);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateArtificialIntelligences();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildArtificialIntelligenceDialogFromNew(aiApi, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateArtificialIntelligences();
        }

        public async void SearchFieldChanged(object? sender, string e)
        {
            if (e.Length < 5)
            {
                return;
            }

            await UpdateArtificialIntelligences();
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
                ? ViewModel.ArtificialIntelligences.OrderBy(item => item.GetSortableValue(propertyName)).ToList()
                : ViewModel.ArtificialIntelligences.OrderByDescending(item => item.GetSortableValue(propertyName))
                    .ToList();

            // Update the ObservableCollection
            ViewModel.ArtificialIntelligences.Clear();
            foreach (var item in sortedItems)
            {
                ViewModel.ArtificialIntelligences.Add(item);
            }
        }

        private string GetPropertyNameFromColumnHeader(string header)
        {
            var converter = new EnumToTitleCaseConverter();
            DataGridColumns column =
                (DataGridColumns) converter.ConvertBack(header, typeof(DataGridColumns), null, null);

            return column switch
            {
                DataGridColumns.NAME => nameof(IArtificialIntelligence.Name),
                DataGridColumns.KEY => nameof(IArtificialIntelligence.Key),
                DataGridColumns.AI_TYPE => nameof(IArtificialIntelligence.AiType),
                _ => throw new ArgumentOutOfRangeException(nameof(header), $"Unexpected column header: {header}")
            };
        }

    }
}
