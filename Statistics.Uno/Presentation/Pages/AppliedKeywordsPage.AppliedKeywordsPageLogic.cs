using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.WinUI.UI.Controls;
using Refit;
using Statistics.Shared.Abstraction.Interfaces.Models;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Pages.ViewModel;
using Statistics.Shared.Extensions;
using Statistics.Uno.Presentation.Core.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Statistics.Uno.Presentation.Core;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class AppliedKeywordsPage
{
    private class AppliedKeywordsPageLogic : BaseLogic<IAppliedKeyword>
    {
        private readonly IAppliedKeywordService appliedKeywordService;
        private readonly IResponseEndpoint responsesApi;
        private readonly IKeywordEndpoint keywordApi;
        private AppliedKeywordsViewModel ViewModel { get; }
        private static IList<IAppliedKeyword>? _appliedKeywordsCache;

        public AppliedKeywordsPageLogic(
            IAppliedKeywordService appliedKeywordService, IResponseEndpoint responsesApi, IKeywordEndpoint keywordApi,
            AppliedKeywordsViewModel viewModel)
        {
            this.appliedKeywordService = appliedKeywordService;
            this.responsesApi = responsesApi;
            this.keywordApi = keywordApi;
            ViewModel = viewModel;
        }

        internal override async Task UpdateDisplayedItems(bool forceUpdate = false)
        {
            if (forceUpdate || _appliedKeywordsCache == null || !_appliedKeywordsCache.Any())
            {
                await UpdateAppliedKeywordsCache();
            }

            ViewModel.AppliedKeywords = _appliedKeywordsCache!.Where(ak => ak.AiType == ViewModel.SelectedAiType).ToObservableCollection();
        }

        private async Task UpdateAppliedKeywordsCache()
        {
            var keywordsResponse = await keywordApi.GetAll(CancellationToken.None);
            EnsureSuccessStatusCode(keywordsResponse);

            var responsesResponse = await responsesApi.GetAll(CancellationToken.None);
            EnsureSuccessStatusCode(responsesResponse);

            _appliedKeywordsCache = (await appliedKeywordService.CalculateAppliedKeywords(
                keywordsResponse.Content ?? throw new InvalidOperationException(),
                responsesResponse.Content ?? throw new InvalidOperationException())).ToList();
        }

        private void EnsureSuccessStatusCode<TEntity>(ApiResponse<List<TEntity>> response)
            where TEntity : class, IEntity
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }

        /// <inheritdoc />
        protected override string GetPropertyNameFromColumnHeader(string header)
        {
            var converter = new EnumToTitleCaseConverter();
            DataGridColumns column =
                (DataGridColumns) converter.ConvertBack(header, typeof(DataGridColumns), null,
                    null);

            return column switch
            {
                DataGridColumns.TEXT => nameof(IAppliedKeyword.Text),
                DataGridColumns.USES_REGULAR_EXPRESSION => nameof(IAppliedKeyword.UsesRegex),
                DataGridColumns.MATCHING_RESPONSES_COUNT => nameof(IAppliedKeyword.MatchingResponsesCount),
                DataGridColumns.TOTAL_RESPONSES_COUNT => nameof(IAppliedKeyword.TotalResponsesCount),
                DataGridColumns.START_SEARCH => nameof(IAppliedKeyword.StartSearch),
                DataGridColumns.END_SEARCH => nameof(IAppliedKeyword.EndSearch),
                _ => throw new ArgumentOutOfRangeException(nameof(header), $"Unexpected column header: {header}")
            };
        }

        /// <inheritdoc />
        protected override ObservableCollection<IAppliedKeyword> GetCollection()
        {
            return ViewModel.AppliedKeywords;
        }
    }
}
