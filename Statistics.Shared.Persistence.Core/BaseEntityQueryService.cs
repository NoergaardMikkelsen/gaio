using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Persistence.Core;

public abstract class BaseEntityQueryService<TContext, TEntity, TSearchable> : IEntityQueryService<TEntity, TSearchable>
    where TContext : BaseDatabaseContext where TEntity : class, IEntity where TSearchable : class, ISearchable, new()
{
    protected readonly TContext context;

    protected BaseEntityQueryService(TContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task AddEntity(TEntity entity, bool saveImmediately = true)
    {
        await context.AddAsync(entity);

        if (saveImmediately)
        {
            await context.SaveChangesAsync();
        }
    }


    /// <inheritdoc />
    public async Task AddEntities(IEnumerable<TEntity> entities, bool saveImmediately = true)
    {
        await context.AddRangeAsync(entities);

        if (saveImmediately)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<TEntity> GetEntity(TSearchable searchable)
    {
        return (await BuildQuery(searchable).ToListAsync()).First();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetEntities(TSearchable searchable)
    {
        return await BuildQuery(searchable).ToListAsync();
    }

    /// <inheritdoc />
    public async Task UpdateEntity(TEntity entity, bool saveImmediately = true)
    {
        context.Update(entity);

        if (saveImmediately)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task UpdateEntities(IEnumerable<TEntity> entities, bool saveImmediately = true)
    {
        context.UpdateRange(entities);

        if (saveImmediately)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task DeleteEntity(TSearchable searchable, bool saveImmediately = true)
    {
        TEntity entity = await GetEntity(searchable);
        context.Remove(entity);

        if (saveImmediately)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task DeleteEntityById(int id, bool saveImmediately = true)
    {
        await DeleteEntity(new TSearchable {Id = id,}, saveImmediately);
    }

    private IQueryable<TEntity> BuildQuery(TSearchable searchable)
    {
        IQueryable<TEntity> query = GetBaseQuery();

        if (searchable.Id != default)
        {
            query = query.Where(x => x.Id == searchable.Id);
        }

        query = AddQueryArguments(searchable, query);

        return query;
    }

    protected abstract IQueryable<TEntity> GetBaseQuery();
    protected abstract IQueryable<TEntity> AddQueryArguments(TSearchable searchable, IQueryable<TEntity> query);
}
