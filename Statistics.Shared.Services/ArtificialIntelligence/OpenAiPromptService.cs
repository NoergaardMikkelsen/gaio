using OpenAI.Chat;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Services.ArtificialIntelligence;

public class OpenAiPromptService : IArtificialIntelligencePromptService
{
    private const string MODEL = "gpt-4o";

    /// <inheritdoc />
    public async Task<IEnumerable<IResponse>> ExecutePrompts(IArtificialIntelligence ai, IEnumerable<IPrompt> prompts)
    {
        var client = new ChatClient(MODEL, ai.Key);

        var promptTasks = prompts.Select(x => client.CompleteChatAsync(x.Text));
        var completedPrompts = await Task.WhenAll(promptTasks);

        return completedPrompts.Select(x => BuildResponseFromChatCompletion(x.Value));
    }

    private Response BuildResponseFromChatCompletion(ChatCompletion chatCompletion)
    {
        return new Response() {Text = chatCompletion.Content[0].Text,};
    }
}
