using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Startup;
using Statistics.Shared.Persistence.Core.Startup;
using Statistics.Uno.Startup;

namespace Statistics.Uno.Persistence.Startup;

public class UnoEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable> :
    BaseEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable>, IUnoStartupModule where TQuery : class, IEntityQueryService<TEntity, TSearchable>
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
