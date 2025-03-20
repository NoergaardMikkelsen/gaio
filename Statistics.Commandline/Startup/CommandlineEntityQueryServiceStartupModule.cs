using Microsoft.Extensions.Hosting;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Persistence.Core.Startup;

namespace Statistics.Commandline.Startup;

public class CommandlineEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable> :
    BaseEntityQueryServiceStartupModule<TQuery, TEntity, TSearchable>, ICommandlineStartupModule
    where TQuery : class, IEntityQueryService<TEntity, TSearchable>
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    /// <inheritdoc />
    public void ConfigureApplication(IHostApplicationBuilder app)
    {

    }
}
