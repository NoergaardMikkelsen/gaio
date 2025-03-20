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
    public ResponseController(IEntityQueryService<Response, SearchableResponse> entityService, ILogger<ResponseController> logger) : base(entityService, logger)
    {
    }
}
