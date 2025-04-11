using OpenAI.Chat;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;

namespace Statistics.Shared.Services.ArtificialIntelligence;

/// <summary>
///     As of 09-04-2025, OpenAI's nuget package with web search capabilities is only in preview.
///     It also has little to no real explanation of how to use it - yet.
///     This service therefore acts just like <see cref="OpenAiNoWebPromptService" />, for now.
/// </summary>
public class OpenAiWebPromptService : BasePromptService, IArtificialIntelligencePromptService
{
    private const string MODEL = "gpt-4o";

    /// <inheritdoc />
    public OpenAiWebPromptService() : base(ArtificialIntelligenceType.OPEN_AI_WEB)
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

    private IResponse BuildResponseFromChatCompletion(
        ChatCompletion chatCompletion, int index, IArtificialIntelligence ai, IEnumerable<IPrompt> prompts)
    {
        return BuildResponse(chatCompletion.Content[0].Text, ai.Id, prompts.ToList()[index].Id);
    }

    private IResponse BuildResponseFromChatCompletion(
        ChatCompletion chatCompletion, IArtificialIntelligence ai, IPrompt prompt)
    {
        return BuildResponse(chatCompletion.Content[0].Text, ai.Id, prompt.Id);
    }
}
