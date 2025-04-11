using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class ArtificialIntelligenceController : EntityController<ArtificialIntelligence,
    SearchableArtificialIntelligence, ArtificialIntelligenceController>
{
    /// <inheritdoc />
    public ArtificialIntelligenceController(
        IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> entityService,
        ILogger<ArtificialIntelligenceController> logger) : base(entityService, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<IEnumerable<ArtificialIntelligence>> GetEntitiesByComplexQuery(
        ComplexSearchable complexSearchable)
    {
        if (complexSearchable.SearchableArtificialIntelligence is null)
        {
            throw new ArgumentNullException(nameof(complexSearchable.SearchableArtificialIntelligence));
        }

        var entities =
            (await entityService.GetEntities(
                (SearchableArtificialIntelligence) complexSearchable.SearchableArtificialIntelligence)).AsEnumerable();

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
