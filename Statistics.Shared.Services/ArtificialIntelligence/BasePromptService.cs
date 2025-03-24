using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Services.ArtificialIntelligence;

public abstract class BasePromptService
{
    private readonly ArtificialIntelligenceType aiServiceType;
    protected BasePromptService(ArtificialIntelligenceType aiServiceType)
    {
        this.aiServiceType = aiServiceType;
    }

    /// <summary>
    /// Checks if the supplied Ai entity is of the correct type.
    /// Throws <exception cref="ArgumentException"/> if the Ai type is incorrect.
    /// </summary>
    /// <param name="ai"></param>
    /// <exception cref="ArgumentException"></exception>
    protected void ValidateSuppliedAi(IArtificialIntelligence ai)
    {
        if (ai.AiType != aiServiceType)
            throw new ArgumentException(
                $"Expected the supplied AI entity of being a '{aiServiceType.ToString()}', but it was a '{ai.AiType.ToString()}'");
    }

    /// <summary>
    /// Builds a fully valid <see cref="Response"/>.
    /// The arguments are all the required ones for an entity that the database will accept. 
    /// </summary>
    /// <param name="text">The response text from the AI.</param>
    /// <param name="aiId">The id of the Ai entity who returned the supplied text.</param>
    /// <param name="promptId">The id of the prompt that the Ai responded to.</param>
    /// <returns></returns>
    protected IResponse BuildResponse(string text, int aiId, int promptId)
    {
        return new Response() {Text = text, AiId = aiId, PromptId = promptId,};
    }
}
