using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Abstraction.Interfaces.Services;

public interface IMasterArtificialIntelligencePromptService
{
    Task<IEnumerable<IResponse>> PromptSuppliedAis(
        IEnumerable<IArtificialIntelligence> ais, IEnumerable<IPrompt> prompts);
}
