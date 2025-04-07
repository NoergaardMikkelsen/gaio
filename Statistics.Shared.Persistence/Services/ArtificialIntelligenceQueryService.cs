using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Services;

public class ArtificialIntelligenceQueryService : BaseEntityQueryService<StatisticsDatabaseContext,
    ArtificialIntelligence, SearchableArtificialIntelligence>
{
    /// <inheritdoc />
    public ArtificialIntelligenceQueryService(StatisticsDatabaseContext context) : base(context)
    {
    }

    /// <inheritdoc />
    protected override IQueryable<ArtificialIntelligence> GetBaseQuery()
    {
        return context.ArtificialIntelligences.AsQueryable().Include(x => x.Responses).ThenInclude(x => x.Prompt);
    }

    /// <inheritdoc />
    protected override IQueryable<ArtificialIntelligence> AddQueryArguments(
        SearchableArtificialIntelligence searchable, IQueryable<ArtificialIntelligence> query)
    {
        if (!string.IsNullOrWhiteSpace(searchable.Key))
        {
            query = query.Where(x => x.Key.ToLower().Contains(searchable.Key.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(searchable.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(searchable.Name.ToLower()));
        }

        return query;
    }
}
