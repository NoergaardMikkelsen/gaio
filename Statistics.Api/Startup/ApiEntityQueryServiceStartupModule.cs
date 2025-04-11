using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Persistence.Core.Startup;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class
    ApiEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable> :
    BaseEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable>,
    IApiStartupModule where TQuery : class, IEntityQueryService<TEntity, TSearchable>
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
