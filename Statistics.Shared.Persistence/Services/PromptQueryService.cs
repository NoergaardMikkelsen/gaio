using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Services;

public class PromptQueryService : BaseEntityQueryService<StatisticsDatabaseContext, Prompt, SearchablePrompt>
{
    /// <inheritdoc />
    public PromptQueryService(StatisticsDatabaseContext context) : base(context)
    {
    }

    /// <inheritdoc />
    protected override IQueryable<Prompt> GetBaseQuery()
    {
        return context.Prompts.AsQueryable().Include(x => x.Responses).ThenInclude(x => x.Ai);
    }

    /// <inheritdoc />
    protected override IQueryable<Prompt> AddQueryArguments(SearchablePrompt searchable, IQueryable<Prompt> query)
    {
        if (!string.IsNullOrWhiteSpace(searchable.Text))
        {
            query = query.Where(x => x.Text.Contains(searchable.Text));
        }

        return query;
    }
}
