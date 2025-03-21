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
}
