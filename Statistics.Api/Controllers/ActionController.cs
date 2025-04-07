using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> aiService;
    private readonly IEntityQueryService<Prompt, SearchablePrompt> promptService;
    private readonly IEntityQueryService<Response, SearchableResponse> responseService;
    private readonly IMasterArtificialIntelligencePromptService masterAiPromptService;
    private readonly ILogger<ActionController> logger;

    public ActionController(
        IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> aiService,
        IEntityQueryService<Prompt, SearchablePrompt> promptService,
        IEntityQueryService<Response, SearchableResponse> responseService,
        IMasterArtificialIntelligencePromptService masterAiPromptService, ILogger<ActionController> logger)
    {
        this.aiService = aiService;
        this.promptService = promptService;
        this.responseService = responseService;
        this.masterAiPromptService = masterAiPromptService;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteAllPrompts()
    {
        try
        {
            var prompts = await promptService.GetEntities(new SearchablePrompt());
            var ais = await aiService.GetEntities(new SearchableArtificialIntelligence());

            var responses = await masterAiPromptService.PromptSuppliedAis(ais, prompts);

            await responseService.AddEntities(responses.Cast<Response>());

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception was caught while attempting to execute all prompts.");
            throw;
        }
    }

    [HttpPost("id")]
    public async Task<IActionResult> ExecutePromptById([FromQuery] int id)
    {
        try
        {
            Prompt prompt = await promptService.GetEntity(new SearchablePrompt() {Id = id,});
            var ais = await aiService.GetEntities(new SearchableArtificialIntelligence());

            var responses = await masterAiPromptService.PromptSuppliedAis(ais, prompt);

            await responseService.AddEntities(responses.Cast<Response>());

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"An exception was caught while attempting to execute prompt with id: {id}.");
            throw;
        }
    }
}
