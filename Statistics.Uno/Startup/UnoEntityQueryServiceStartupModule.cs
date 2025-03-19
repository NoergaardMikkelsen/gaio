using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Persistence.Core.Startup;

namespace Statistics.Uno.Startup;

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
