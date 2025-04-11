using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class ResponseController : EntityController<Response, SearchableResponse, ResponseController>
{
    /// <inheritdoc />
    public ResponseController(
        IEntityQueryService<Response, SearchableResponse> entityService, ILogger<ResponseController> logger) : base(
        entityService, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<IEnumerable<Response>> GetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
    {
        if (complexSearchable.SearchableResponse is null)
        {
            throw new ArgumentNullException(nameof(complexSearchable.SearchableResponse));
        }

        var entities = (await entityService.GetEntities((SearchableResponse) complexSearchable.SearchableResponse))
            .AsEnumerable();

        if (complexSearchable.SearchableArtificialIntelligence != null)
        {
            if (!string.IsNullOrWhiteSpace(complexSearchable.SearchableArtificialIntelligence.Name))
            {
                entities = entities.Where(x =>
                    x.Ai.Name.Contains(complexSearchable.SearchableArtificialIntelligence.Name,
                        StringComparison.InvariantCultureIgnoreCase));
            }

            if (complexSearchable.SearchableArtificialIntelligence.AiType != null)
            {
                entities = entities.Where(x =>
                    x.Ai.AiType == complexSearchable.SearchableArtificialIntelligence.AiType);
            }
        }

        if (complexSearchable.SearchablePrompt != null &&
            !string.IsNullOrWhiteSpace(complexSearchable.SearchablePrompt.Text))
        {
            entities = entities.Where(x => x.Prompt.Text.Contains(complexSearchable.SearchablePrompt.Text,
                StringComparison.InvariantCultureIgnoreCase));
        }

        return entities;
    }
}
