using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Statistics.Api.Hubs;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Api.Controllers;

public abstract class EntityController<TEntity, TSearchable, TController> : ControllerBase
    where TEntity : class, IEntity where TSearchable : class, ISearchable, new() where TController : ControllerBase
{
    private readonly IEntityQueryService<TEntity, TSearchable> entityService;
    private readonly ILogger<TController> logger;
    private readonly IHubContext<NotificationHub, INotificationHub> hubContext;
    private readonly SignalrEvent entitySignalrEventType;

    protected EntityController(
        IEntityQueryService<TEntity, TSearchable> entityService, ILogger<TController> logger,
        IHubContext<NotificationHub, INotificationHub> hubContext, SignalrEvent entitySignalrEventType)
    {
        this.entityService = entityService;
        this.logger = logger;
        this.hubContext = hubContext;
        this.entitySignalrEventType = entitySignalrEventType;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var entities = await entityService.GetEntities(new TSearchable());
            return Ok(entities);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception was caught while attempting to get all entities of the controllers type.");
            throw;
        }
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            IEntity entity = await entityService.GetEntity(new TSearchable {Id = id,});
            return Ok(entity);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to get an entity by id of the controllers type.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetByQuery([FromBody] TSearchable searchable)
    {
        try
        {
            IEntity entity = await entityService.GetEntity(searchable);
            return Ok(entity);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to get entities matching specified query of the controllers type.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetAllByQuery([FromBody] TSearchable searchable)
    {
        try
        {
            var entities = await entityService.GetEntities(searchable);
            return Ok(entities);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to get entities matching specified query of the controllers type.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddSingle([FromBody] TEntity entity)
    {
        try
        {
            await entityService.AddEntity(entity);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity of type {nameof(TEntity)} was added.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to add a single entity of the controllers type.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddMultiple([FromBody] IEnumerable<TEntity> entities)
    {
        try
        {
            await entityService.AddEntities(entities);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity or more of type {nameof(TEntity)} was added.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to add multiple entities of the controllers type.");
            throw;
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSingle([FromBody] TEntity entity)
    {
        try
        {
            await entityService.UpdateEntity(entity);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity of type {nameof(TEntity)} was updated.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to update a specific entity of the controllers type.");
            throw;
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMultiple([FromBody] IEnumerable<TEntity> entities)
    {
        try
        {
            await entityService.UpdateEntities(entities);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity or more of type {nameof(TEntity)} was updated.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to update multiple entities of the controllers type.");
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteByQuery([FromBody] TSearchable searchable)
    {
        try
        {
            await entityService.DeleteEntity(searchable);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity of type {nameof(TEntity)} was deleted.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to delete a specific entity by specified query of the controllers type.");
            throw;
        }
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteById(int id)
    {
        try
        {
            await entityService.DeleteEntityById(id);
            await hubContext.Clients.All.SendEntityChangedNotification(entitySignalrEventType,
                $"An entity of type {nameof(TEntity)} was deleted.");
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to delete a entity by id of the controllers type.");
            throw;
        }
    }
}
