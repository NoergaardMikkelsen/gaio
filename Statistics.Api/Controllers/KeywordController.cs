using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class KeywordController : EntityController<Keyword, SearchableKeyword, KeywordController>
{
    /// <inheritdoc />
    public KeywordController(IEntityQueryService<Keyword, SearchableKeyword> entityService, ILogger<KeywordController> logger) : base(entityService, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<IEnumerable<Keyword>> GetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
    {
        if (complexSearchable.SearchableKeyword is null)
            throw new ArgumentNullException(nameof(complexSearchable.SearchableKeyword));

        var entities = (await entityService.GetEntities((SearchableKeyword) complexSearchable.SearchableKeyword))
            .AsEnumerable();

        return entities;
    }
}
