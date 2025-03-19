using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Shared.Persistence.Core.Startup;

public abstract class BaseEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable> : IStartupModule
    where TQuery : class, IEntityQueryService<TEntity, TSearchable>
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IEntityQueryService<TEntity, TSearchable>, TQuery>();
    }
}
