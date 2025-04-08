using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Extensions;

namespace Statistics.Shared.Services.ArtificialIntelligence;

public class MasterArtificialIntelligencePromptService : IMasterArtificialIntelligencePromptService
{
    private readonly Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService> promptServices;

    public MasterArtificialIntelligencePromptService(
        Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService>? promptServices = null)
    {
        this.promptServices = promptServices ??
                              new Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService>()
                              {
                                  {ArtificialIntelligenceType.OPEN_AI_NO_WEB, new OpenAiNoWebPromptService()},
                                    {ArtificialIntelligenceType.OPEN_AI_WEB, new OpenAiWebPromptService()},
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

        return (await Task.WhenAll(tasks)).Flatten();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IResponse>> PromptSuppliedAis(IEnumerable<IArtificialIntelligence> ais, IPrompt prompt)
    {
        var tasks = new List<Task<IResponse>>();

        foreach (IArtificialIntelligence ai in ais)
        {
            IArtificialIntelligencePromptService service = promptServices.First(pair => pair.Key == ai.AiType).Value;
            tasks.Add(service.ExecutePrompt(ai, prompt));
        }

        return await Task.WhenAll(tasks);
    }
}
