using Refit;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Uno.Endpoints;

[Headers("Content-Type: application/json")]
public interface IEntityEndpoint<TEntity, TSearchable> : IRefitEndpoint where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    [Post("/GetAllByQuery")]
    Task<ApiResponse<TEntity>> GetAllByQuery(CancellationToken ct, [Body] TSearchable searchable);

    [Post("/GetByQuery")]
    Task<ApiResponse<TEntity>> GetByQuery(CancellationToken ct, [Body] TSearchable searchable);

    [Get("/GetAll")]
    Task<ApiResponse<List<TEntity>>> GetAll(CancellationToken ct);

    [Delete("/DeleteById/{id}")]
    Task<ApiResponse<bool>> DeleteById(CancellationToken ct, int id);
}
