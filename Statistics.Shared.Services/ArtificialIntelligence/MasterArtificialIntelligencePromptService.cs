using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;

namespace Statistics.Shared.Services.ArtificialIntelligence;

public class MasterArtificialIntelligencePromptService : IMasterArtificialIntelligencePromptService
{
    private readonly Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService> promptServices;

    public MasterArtificialIntelligencePromptService()
    {
        promptServices = new Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService>()
        {
            {ArtificialIntelligenceType.OPEN_AI, new OpenAiPromptService()},
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IResponse>> PromptSuppliedAis(
        IEnumerable<IArtificialIntelligence> ais, IEnumerable<IPrompt> prompts)
    {
        var tasks = new List<Task<IEnumerable<IResponse>>>();
        var enumeratedPrompts = prompts.ToList();

        foreach (IArtificialIntelligence ai in ais)
        {
            IArtificialIntelligencePromptService service = promptServices.First(pair => pair.Key == ai.AiType).Value;
            tasks.Add(service.ExecutePrompts(ai, enumeratedPrompts));
        }

        return (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
    }
}
