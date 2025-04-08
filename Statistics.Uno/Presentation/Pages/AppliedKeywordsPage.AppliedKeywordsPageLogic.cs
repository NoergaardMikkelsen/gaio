using System.Net;
using Refit;
using Statistics.Shared.Abstraction.Interfaces.Models;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class AppliedKeywordsPage
{
    private class AppliedKeywordsPageLogic
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
            ViewModel.AppliedKeywords = new List<IAppliedKeyword>();
        }

        internal async Task UpdateAppliedKeywords(bool forceUpdate = false)
        {
            if (forceUpdate || _appliedKeywordsCache == null || !_appliedKeywordsCache.Any())
            {
                await UpdateAppliedKeywordsCache();
            }

            ViewModel.AppliedKeywords = _appliedKeywordsCache!.Where(ak => ak.AiType == ViewModel.SelectedAiType).ToList();
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
    }
}
