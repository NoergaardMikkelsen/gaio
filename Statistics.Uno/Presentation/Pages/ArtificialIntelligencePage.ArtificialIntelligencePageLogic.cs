using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
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
            
        }

        // Helper method to get the value of a property by name
        private static object? GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }
    }
}
