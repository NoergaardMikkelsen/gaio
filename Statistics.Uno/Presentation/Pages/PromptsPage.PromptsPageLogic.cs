using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;
using Statistics.Uno.Presentation.Factory;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class PromptsPage
{
    private class PromptsPageLogic : BaseLogic<IPrompt>
    {
        private readonly IPromptEndpoint promptApi;
        private readonly IActionEndpoint actionApi;
        private readonly Page page;
        private CancellationTokenSource updateCancellationTokenSource;
        private PromptsViewModel ViewModel { get; }

        public PromptsPageLogic(
            IPromptEndpoint promptApi, IActionEndpoint actionApi, PromptsViewModel viewModel, PromptsPage page)
        {
            ViewModel = viewModel;
            this.promptApi = promptApi;
            this.actionApi = actionApi;
            this.page = page;
            updateCancellationTokenSource = new CancellationTokenSource();
        }

        internal override async Task UpdateDisplayedItems(bool forceUpdate = false)
        {
            ISearchablePrompt searchable = BuildSearchablePrompt();

            try
            {
                await updateCancellationTokenSource.CancelAsync();
                updateCancellationTokenSource = new CancellationTokenSource();

                Console.WriteLine($"Updating Prompts...");
                ViewModel.UpdatingText = "Updating...";

                var apiResponse = await promptApi.GetAllByQuery(updateCancellationTokenSource.Token,
                    (SearchablePrompt) searchable);

                if (!apiResponse.IsSuccessful)
                {
                    Console.WriteLine($"Request to Api was not successful. Error is as follows: {apiResponse.Error}");
                    return;
                }

                var allPrompts = apiResponse.Content;

                if (allPrompts == null)
                {
                    Console.WriteLine($"Failed to get all prompt entities.");
                    return;
                }

                ViewModel.Prompts = new ObservableCollection<IPrompt>(allPrompts);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Update of Prompts Cancelled...");
            }
            finally
            {
                ViewModel.UpdatingText = string.Empty;
            }
        }

        private ISearchablePrompt BuildSearchablePrompt()
        {
            return new SearchablePrompt()
            {
                Text = ViewModel.SearchablePromptText ?? string.Empty,
            };
        }

        public async void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var promptId = (int) button.Tag;

            IPrompt prompt = ViewModel.Prompts.FirstOrDefault(x => x.Id == promptId) ??
                             throw new NullReferenceException(
                                 $"Expected to find a prompt with id '{promptId}', but it was not found.");

            await ContentDialogFactory.ShowBuildPromptDialogFromExisting(promptApi, prompt, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateDisplayedItems();
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

            var promptId = (int) button.Tag;

            await promptApi.DeleteById(CancellationToken.None, promptId);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await UpdateDisplayedItems();
        }

        public async void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ContentDialogFactory.ShowBuildPromptDialogFromNew(promptApi, page.XamlRoot);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await UpdateDisplayedItems();
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
        }

        public async void ExecutePromptOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button ??
                            throw new NullReferenceException(
                                $"Expected '{nameof(sender)}' to not be null, but it was.");

            var promptId = (int) button.Tag;

            button.Content = "Executing...";
            button.IsEnabled = false;
            await actionApi.ExecutePromptById(CancellationToken.None, promptId);
            button.Content = "Execute";
            button.IsEnabled = true;
        }

        /// <inheritdoc />
        protected override string GetPropertyNameFromColumnHeader(string header)
        {
            var converter = new EnumToTitleCaseConverter();
            DataGridColumns column =
                (DataGridColumns) converter.ConvertBack(header, typeof(DataGridColumns), null, null);

            return column switch
            {
                DataGridColumns.PROMPT_TEXT => nameof(IPrompt.Text),
                DataGridColumns.CREATED_AT => nameof(IPrompt.CreatedDateTime),
                DataGridColumns.LAST_UPDATED_AT => nameof(IPrompt.UpdatedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(header), $"Unexpected column header: {header}"),
            };
        }

        /// <inheritdoc />
        protected override ObservableCollection<IPrompt> GetCollection()
        {
            return ViewModel.Prompts;
        }


    }
}
