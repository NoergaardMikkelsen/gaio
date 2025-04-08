using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class KeywordsPage
{
    private class KeywordsPageLogic
    {
        private readonly IKeywordEndpoint keywordApi;
        private readonly Page page;
        private CancellationTokenSource updateCancellationTokenSource;
        private KeywordsViewModel ViewModel { get; }

        public KeywordsPageLogic(IKeywordEndpoint keywordApi, KeywordsViewModel viewModel, Page page)
        {
            ViewModel = viewModel;
            this.keywordApi = keywordApi;
            this.page = page;
            updateCancellationTokenSource = new CancellationTokenSource();
        }

        internal async Task UpdateKeywords()
        {
            ISearchableKeyword searchable = BuildSearchableKeyword();

            try
            {
                await updateCancellationTokenSource.CancelAsync();
                updateCancellationTokenSource = new CancellationTokenSource();

                Console.WriteLine($"Updating Keywords...");
                ViewModel.UpdatingText = "Updating...";

                var apiResponse = await keywordApi.GetAllByQuery(updateCancellationTokenSource.Token,
                    (SearchableKeyword) searchable);

                if (!apiResponse.IsSuccessful)
                {
                    Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                    return;
                }

                var allKeywords = apiResponse.Content;

                if (allKeywords == null)
                {
                    Console.WriteLine($"Failed to get all keyword entities.");
                    return;
                }

                ViewModel.Keywords = new ObservableCollection<IKeyword>(allKeywords);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Update of Keywords Cancelled...");
            }
            finally
            {
                ViewModel.UpdatingText = string.Empty;
            }
        }

        private ISearchableKeyword BuildSearchableKeyword()
        {
            return new SearchableKeyword()
            {
                Text = ViewModel.SearchableKeywordText ?? string.Empty,
            };
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var keywordId = (int) button.Tag;

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

            var keywordId = (int) button.Tag;

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

        public async void SearchFieldChanged(object? sender, string e)
        {
            if (e.Length < 5)
            {
                return;
            }

            await UpdateKeywords();
        }

        public void SortItems(object? sender, DataGridColumnEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
