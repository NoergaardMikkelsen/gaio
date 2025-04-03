using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models;

namespace Statistics.Shared.Models;

public class AppliedKeyword : IAppliedKeyword
{
    /// <inheritdoc />
    public string Text { get; set; }

    /// <inheritdoc />
    public bool UsesRegex { get; set; }

    /// <inheritdoc />
    public ArtificialIntelligenceType AiType { get; set; }

    /// <inheritdoc />
    public int TotalResponsesCount { get; set; }

    /// <inheritdoc />
    public int MatchingResponsesCount { get; set; }

    /// <inheritdoc />
    public DateTime? StartSearch { get; set; }

    /// <inheritdoc />
    public DateTime? EndSearch { get; set; }
}
