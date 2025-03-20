using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class PromptController : EntityController<Prompt, SearchablePrompt, PromptController>
{
    /// <inheritdoc />
    public PromptController(IEntityQueryService<Prompt, SearchablePrompt> entityService, ILogger<PromptController> logger) : base(entityService, logger)
    {
    }
}
