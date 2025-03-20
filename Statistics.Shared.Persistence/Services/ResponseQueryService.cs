using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Services;

public class ResponseQueryService : BaseEntityQueryService<StatisticsDatabaseContext, Response, SearchableResponse>
{
    /// <inheritdoc />
    public ResponseQueryService(StatisticsDatabaseContext context) : base(context)
    {
    }

    /// <inheritdoc />
    protected override IQueryable<Response> GetBaseQuery()
    {
        return context.Responses.AsQueryable();
    }

    /// <inheritdoc />
    protected override IQueryable<Response> AddQueryArguments(SearchableResponse searchable, IQueryable<Response> query)
    {
        if (searchable.AiId != default)
            query = query.Where(x => x.AiId == searchable.AiId);
        if (searchable.PromptId != default)
            query = query.Where(x => x.PromptId == searchable.PromptId);
        if (!string.IsNullOrWhiteSpace(searchable.Text))
            query = query.Where(x => x.Text.Contains(searchable.Text));

        return query;
    }
}
