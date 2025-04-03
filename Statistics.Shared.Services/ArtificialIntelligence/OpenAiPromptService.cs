using OpenAI.Chat;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;

namespace Statistics.Shared.Services.ArtificialIntelligence;

public class OpenAiPromptService : BasePromptService, IArtificialIntelligencePromptService
{
    private const string MODEL = "gpt-4o";

    /// <inheritdoc />
    public OpenAiPromptService() : base(ArtificialIntelligenceType.OPEN_AI)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IResponse>> ExecutePrompts(IArtificialIntelligence ai, IEnumerable<IPrompt> prompts)
    {
        ValidateSuppliedAi(ai);

        var client = new ChatClient(MODEL, ai.Key);

        var promptList = prompts.ToList();
        var promptTasks = promptList.Select(x => client.CompleteChatAsync(x.Text));
        var completedPrompts = await Task.WhenAll(promptTasks);

        return completedPrompts.Select((x, index) => BuildResponseFromChatCompletion(x.Value, index, ai, promptList));
    }

    /// <inheritdoc />
    public async Task<IResponse> ExecutePrompt(IArtificialIntelligence ai, IPrompt prompt)
    {
        ValidateSuppliedAi(ai);

        var client = new ChatClient(MODEL, ai.Key);

        var chatCompletion = await client.CompleteChatAsync(prompt.Text);

        return BuildResponseFromChatCompletion(chatCompletion, ai, prompt);
    }

    private IResponse BuildResponseFromChatCompletion(ChatCompletion chatCompletion, int index, IArtificialIntelligence ai, IEnumerable<IPrompt> prompts)
    {
        return BuildResponse(chatCompletion.Content[0].Text, ai.Id, prompts.ToList()[index].Id);
    }

    private IResponse BuildResponseFromChatCompletion(ChatCompletion chatCompletion, IArtificialIntelligence ai, IPrompt prompt)
    {
        return BuildResponse(chatCompletion.Content[0].Text, ai.Id, prompt.Id);
    }
}
