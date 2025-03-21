using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Api.Controllers;

public abstract class EntityController<TEntity, TSearchable, TController> : ControllerBase
    where TEntity : class, IEntity where TSearchable : class, ISearchable, new() where TController : ControllerBase
{
    private readonly IEntityQueryService<TEntity, TSearchable> entityService;
    private readonly ILogger<TController> logger;

    protected EntityController(IEntityQueryService<TEntity, TSearchable> entityService, ILogger<TController> logger)
    {
        this.entityService = entityService;
        this.logger = logger;
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
            IEntity payment = await entityService.GetEntity(new TSearchable {Id = id,});
            return Ok(payment);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to get an entity by id of the controllers type.");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByQuery([FromBody] TSearchable searchable)
    {
        try
        {
            IEntity payment = await entityService.GetEntity(searchable);
            return Ok(payment);
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
