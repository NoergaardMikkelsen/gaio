using Statistics.Shared.Abstraction.Enum;

namespace Statistics.Shared.Abstraction.Interfaces.Models;

public interface IAppliedKeyword
{
    public string Text { get; set; }
    public bool UsesRegex { get; set; }
    public ArtificialIntelligenceType AiType { get; set; }
    public int TotalResponsesCount { get; set; }
    public int MatchingResponsesCount { get; set; }
    public DateTime? StartSearch { get; set; }
    public DateTime? EndSearch { get; set; }
}
