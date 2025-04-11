using Microsoft.AspNetCore.Mvc;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Api.Controllers;

public abstract class EntityController<TEntity, TSearchable, TController> : ControllerBase
    where TEntity : class, IEntity where TSearchable : class, ISearchable, new() where TController : ControllerBase
{
    protected readonly IEntityQueryService<TEntity, TSearchable> entityService;
    private readonly ILogger<TController> logger;

    protected EntityController(IEntityQueryService<TEntity, TSearchable> entityService, ILogger<TController> logger)
    {
        this.entityService = entityService;
        this.logger = logger;
    }

    protected abstract Task<IEnumerable<TEntity>> GetEntitiesByComplexQuery(ComplexSearchable complexSearchable);

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
                "An exception was caught while attempting to get an entity by id of the controllers type. Id: {Id}",
                id);
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
                "An exception was caught while attempting to get entities matching specified query of the controllers type. Query: {@Searchable}",
                searchable);
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
                "An exception was caught while attempting to get entities matching specified query of the controllers type. Query: {@Searchable}",
                searchable);
            throw;
        }
    }


    /// <summary>
    ///     Get all entities matching specified complex query of the controllers type.
    ///     If Searchable properties for child entities are supplied, then it will also limit result by those.
    /// </summary>
    /// <param name="complexSearchable"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> GetAllByComplexQuery([FromBody] ComplexSearchable complexSearchable)
    {
        try
        {
            var entities = await GetEntitiesByComplexQuery(complexSearchable);
            return Ok(entities);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception was caught while attempting to get entities matching specified complex query of the controllers type. Complex Query: {@ComplexSearchable}",
                complexSearchable);
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
                "An exception was caught while attempting to add a single entity of the controllers type. Entity: {@Entity}",
                entity);
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
                "An exception was caught while attempting to add multiple entities of the controllers type. Entities: {@Entities}",
                entities);
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
                "An exception was caught while attempting to update a specific entity of the controllers type. Entity: {@Entity}",
                entity);
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
                "An exception was caught while attempting to update multiple entities of the controllers type. Entities: {@Entities}",
                entities);
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
                "An exception was caught while attempting to delete a specific entity by specified query of the controllers type. Query: {@Searchable}",
                searchable);
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
                "An exception was caught while attempting to delete an entity by id of the controllers type. Id: {Id}",
                id);
            throw;
        }
    }
}
