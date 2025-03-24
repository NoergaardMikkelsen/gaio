using Refit;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Uno.Endpoints;

[Headers("Content-Type: application/json")]
public interface IResponsesEndpoint : IRefitEndpoint
{
    [Get("/GetAllByQuery")]
    Task<ApiResponse<Response>> GetAllByQuery(CancellationToken ct, [Body] SearchableResponse searchable);
}
