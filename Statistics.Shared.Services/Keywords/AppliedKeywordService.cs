using System.Text.RegularExpressions;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models;

namespace Statistics.Shared.Services.Keywords;

public class AppliedKeywordService : IAppliedKeywordService
{
    /// <inheritdoc />
    public async Task<IEnumerable<IAppliedKeyword>> CalculateAppliedKeywords(
        IEnumerable<IKeyword> keywords, IEnumerable<IResponse> responses)
    {
        var types = Enum.GetValues<ArtificialIntelligenceType>();

        var generateAppliedKeywordsTasks = types.Select((type) =>
            GenerateAppliedKeywordsForAiType(type, keywords, responses));

        var appliedKeywordsByType = await Task.WhenAll(generateAppliedKeywordsTasks);

        return appliedKeywordsByType.Flatten();
    }

    private async Task<IEnumerable<IAppliedKeyword>> GenerateAppliedKeywordsForAiType(
        ArtificialIntelligenceType type, IEnumerable<IKeyword> keywords, IEnumerable<IResponse> responses)
    {
        var typeMatchingResponses = responses.Where(x => x.Ai.AiType == type).ToList();

        var generateAppliedKeywordsTasks = keywords.AsParallel().Select((word) =>
            word.UseRegex
                ? GenerateAppliedKeywordUsingRegex(type, word, typeMatchingResponses)
                : GenerateAppliedKeywordUsingContains(type, word, typeMatchingResponses));

        return await Task.WhenAll(generateAppliedKeywordsTasks);
    }

    private Task<AppliedKeyword> GenerateAppliedKeywordUsingRegex(
        ArtificialIntelligenceType type, IKeyword keyword, IEnumerable<IResponse> responses)
    {
        var enumeratedResponses = responses.ToList();
        var regex = new Regex(keyword.Text, RegexOptions.Compiled);

        if (keyword is {StartSearch: not null, EndSearch: not null,})
        {
            return Task.FromResult(
                BuildAppliedKeywordForTimeRangeUsingRegex(type, keyword, enumeratedResponses, regex));
        }

        if (keyword.StartSearch != null)
        {
            return Task.FromResult(
                BuildAppliedKeywordWithStartOnlyUsingRegex(type, keyword, enumeratedResponses, regex));
        }

        if (keyword.EndSearch != null)
        {
            return Task.FromResult(BuildAppliedKeywordWithEndOnlyUsingRegex(type, keyword, enumeratedResponses, regex));
        }

        return Task.FromResult(BuildAppliedKeywordUsingRegex(type, keyword, enumeratedResponses, regex));
    }

    private AppliedKeyword BuildAppliedKeywordUsingRegex(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses, Regex regex)
    {
        var matchingResponses = responses.AsParallel().Where(response => regex.IsMatch(response.Text));

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordWithEndOnlyUsingRegex(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses, Regex regex)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            regex.IsMatch(response.Text) && response.CreatedDateTime <= keyword.EndSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordWithStartOnlyUsingRegex(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses, Regex regex)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            regex.IsMatch(response.Text) && response.CreatedDateTime >= keyword.StartSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordForTimeRangeUsingRegex(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses, Regex regex)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            regex.IsMatch(response.Text) && response.CreatedDateTime >= keyword.StartSearch &&
            response.CreatedDateTime <= keyword.EndSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private Task<AppliedKeyword> GenerateAppliedKeywordUsingContains(
        ArtificialIntelligenceType type, IKeyword keyword, IEnumerable<IResponse> responses)
    {
        var enumeratedResponses = responses.ToList();

        if (keyword is {StartSearch: not null, EndSearch: not null,})
        {
            return Task.FromResult(BuildAppliedKeywordForTimeRangeUsingContains(type, keyword, enumeratedResponses));
        }

        if (keyword.StartSearch != null)
        {
            return Task.FromResult(BuildAppliedKeywordWithStartOnlyUsingContains(type, keyword, enumeratedResponses));
        }

        if (keyword.EndSearch != null)
        {
            return Task.FromResult(BuildAppliedKeywordWithEndOnlyUsingContains(type, keyword, enumeratedResponses));
        }

        return Task.FromResult(BuildAppliedKeywordUsingContains(type, keyword, enumeratedResponses));
    }

    private AppliedKeyword BuildAppliedKeywordUsingContains(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses)
    {
        var matchingResponses = responses.AsParallel().Where(response => response.Text.Contains(keyword.Text));

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordWithEndOnlyUsingContains(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            response.Text.Contains(keyword.Text) && response.CreatedDateTime <= keyword.EndSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordWithStartOnlyUsingContains(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            response.Text.Contains(keyword.Text) && response.CreatedDateTime >= keyword.StartSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }

    private AppliedKeyword BuildAppliedKeywordForTimeRangeUsingContains(
        ArtificialIntelligenceType type, IKeyword keyword, IList<IResponse> responses)
    {
        var matchingResponses = responses.AsParallel().Where(response =>
            response.Text.Contains(keyword.Text) && response.CreatedDateTime >= keyword.StartSearch &&
            response.CreatedDateTime <= keyword.EndSearch);

        return new AppliedKeyword
        {
            AiType = type,
            Text = keyword.Text,
            StartSearch = keyword.StartSearch,
            EndSearch = keyword.EndSearch,
            MatchingResponsesCount = matchingResponses.Count(),
            TotalResponsesCount = responses.Count,
        };
    }
}
