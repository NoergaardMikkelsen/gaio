using Refit;

namespace Statistics.Shared.Abstraction.Interfaces.Refit;

[Headers("Content-Type: application/json")]
public interface IActionEndpoint : IRefitEndpoint
{
    [Post("/ExecuteAllPrompts")]
    Task<ApiResponse<bool>> ExecuteAllPrompts(CancellationToken ct);

    [Post("/ExecutePromptById/id")]
    Task<ApiResponse<bool>> ExecutePromptById(CancellationToken ct, [Query] int id);
}
