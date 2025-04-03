using Statistics.Shared.Abstraction.Interfaces.Models;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Abstraction.Interfaces.Services;

public interface IAppliedKeywordService
{
    public Task<IEnumerable<IAppliedKeyword>> CalculateAppliedKeywords(
        IEnumerable<IKeyword> keywords, IEnumerable<IResponse> responses);
}
