using Statistics.Shared.Abstraction.Enum;

namespace Statistics.Shared.Models;

public class AppliedKeyword
{
    public string Text { get; set; }
    public ArtificialIntelligenceType AiType { get; set; }
    public int TotalResponsesCount { get; set; }
    public int MatchingResponsesCount { get; set; }
    public DateTime? StartSearch { get; set; }
    public DateTime? EndSearch { get; set; }
}
