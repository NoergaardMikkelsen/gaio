using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Statistics.Api.Hubs;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

[Route(Constants.ROUTE_TEMPLATE)]
[ApiController]
public class KeywordController : EntityController<Keyword, SearchableKeyword, KeywordController>
{
    /// <inheritdoc />
    public KeywordController(
        IEntityQueryService<Keyword, SearchableKeyword> entityService, ILogger<KeywordController> logger,
        IHubContext<NotificationHub, INotificationHub> hubContext) : base(entityService, logger, hubContext,
        SignalrEvent.KEYWORDS_CHANGED)
    {
    }
}
