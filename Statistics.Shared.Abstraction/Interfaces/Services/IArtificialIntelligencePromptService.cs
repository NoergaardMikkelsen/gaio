using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Abstraction.Interfaces.Services;

public interface IArtificialIntelligencePromptService
{
    Task<IEnumerable<IResponse>> ExecutePrompts(IArtificialIntelligence ai, IEnumerable<IPrompt> prompts);
    Task<IResponse> ExecutePrompt(IArtificialIntelligence ai, IPrompt prompt);
}
