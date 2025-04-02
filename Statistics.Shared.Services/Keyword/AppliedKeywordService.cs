using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Services.Keyword;

public class AppliedKeywordService
{
    public AppliedKeywordService()
    {
        
    }

    public IEnumerable<AppliedKeyword> CalculateAppliedKeywords(
        IEnumerable<Models.Entity.Keyword> keywords, IEnumerable<Response> responses)
    {
        var types = Enum.GetValues<ArtificialIntelligenceType>();
        var appliedKeywords = new List<AppliedKeyword>();
        var enumerable = keywords.ToList();


        foreach (ArtificialIntelligenceType type in types)
        {
            var typedAppliedKeywords = enumerable.Select((word) =>
            {
                if (word.UseRegex)
                    return BuildAppliedKeywordFromRegex(type, word, responses);

                if (word is {StartSearch: not null, EndSearch: not null})
                    return BuildAppliedKeywordForTimeRange(type, word, responses);

                if (word.EndSearch != null)
                    return BuildAppliedKeywordWithEndOnly(type, word, responses);

                if (word.StartSearch != null)
                    return BuildAppliedKeywordWithStartOnly(type, word, responses);

                return BuildSimpleAppliedKeyword(type, word, responses);
            });

            appliedKeywords.AddRange(typedAppliedKeywords);
        }

        return appliedKeywords;
    }

    private AppliedKeyword BuildSimpleAppliedKeyword(ArtificialIntelligenceType type, Models.Entity.Keyword word, IEnumerable<Response> responses)
    {
        throw new NotImplementedException();
    }

    private AppliedKeyword BuildAppliedKeywordWithStartOnly(ArtificialIntelligenceType type, Models.Entity.Keyword word, IEnumerable<Response> responses)
    {
        throw new NotImplementedException();
    }

    private AppliedKeyword BuildAppliedKeywordWithEndOnly(ArtificialIntelligenceType type, Models.Entity.Keyword word, IEnumerable<Response> responses)
    {
        throw new NotImplementedException();
    }

    private AppliedKeyword BuildAppliedKeywordForTimeRange(ArtificialIntelligenceType type, Models.Entity.Keyword word, IEnumerable<Response> responses)
    {
        throw new NotImplementedException();
    }

    private AppliedKeyword BuildAppliedKeywordFromRegex(ArtificialIntelligenceType type, Models.Entity.Keyword word, IEnumerable<Response> responses)
    {
        throw new NotImplementedException();
    }
}
