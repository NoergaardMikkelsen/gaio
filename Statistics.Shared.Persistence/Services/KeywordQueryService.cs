using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Services;

public class KeywordQueryService : BaseEntityQueryService<StatisticsDatabaseContext, Keyword, SearchableKeyword>
{
    /// <inheritdoc />
    public KeywordQueryService(StatisticsDatabaseContext context) : base(context)
    {
    }

    /// <inheritdoc />
    protected override IQueryable<Keyword> GetBaseQuery()
    {
        return context.Keywords.AsQueryable();
    }

    /// <inheritdoc />
    protected override IQueryable<Keyword> AddQueryArguments(SearchableKeyword searchable, IQueryable<Keyword> query)
    {
        if (!string.IsNullOrWhiteSpace(searchable.Text))
        {
            query = query.Where(x => x.Text.ToLower().Contains(searchable.Text.ToLower()));
        }

        return query;
    }
}
