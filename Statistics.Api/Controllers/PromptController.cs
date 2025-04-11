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
    public PromptController(
        IEntityQueryService<Prompt, SearchablePrompt> entityService, ILogger<PromptController> logger) : base(
        entityService, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<IEnumerable<Prompt>> GetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
    {
        if (complexSearchable.SearchablePrompt is null)
        {
            throw new ArgumentNullException(nameof(complexSearchable.SearchablePrompt));
        }

        var entities = (await entityService.GetEntities((SearchablePrompt) complexSearchable.SearchablePrompt))
            .AsEnumerable();

        if (complexSearchable.SearchableResponse != null &&
            !string.IsNullOrWhiteSpace(complexSearchable.SearchableResponse.Text))
        {
            entities = entities.Where(x => x.Responses.Any(y =>
                y.Text.Contains(complexSearchable.SearchableResponse.Text,
                    StringComparison.InvariantCultureIgnoreCase)));
        }

        return entities;
    }
}
