using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Service;

[TestFixture]
public class ArtificialIntelligenceQueryServiceTests
{
    private StatisticsDatabaseContext _context;
    private TestableArtificialIntelligenceQueryService _service;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.ArtificialIntelligences.AddRange(
            new ArtificialIntelligence() { Name = "AI 1", Key = "Key1", AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB},
            new ArtificialIntelligence() { Name = "AI 2", Key = "Key2",
                AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB
            }
        );
        _context.SaveChanges();

        // Initialize testable service
        _service = new TestableArtificialIntelligenceQueryService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void GetBaseQuery_ShouldReturnAllArtificialIntelligencesWithResponsesAndPrompts()
    {
        // Act
        var result = _service.TestGetBaseQuery().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("AI 1");
        result[1].Name.Should().Be("AI 2");
    }

    [Test]
    public void AddQueryArguments_ShouldFilterByNameAndKey()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence { Name = "AI 1", Key = "Key1" };
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("AI 1");
        result[0].Key.Should().Be("Key1");
    }

    private class TestableArtificialIntelligenceQueryService : ArtificialIntelligenceQueryService
    {
        public TestableArtificialIntelligenceQueryService(StatisticsDatabaseContext context) : base(context)
        {
        }

        public IQueryable<ArtificialIntelligence> TestGetBaseQuery()
        {
            return GetBaseQuery();
        }

        public IQueryable<ArtificialIntelligence> TestAddQueryArguments(SearchableArtificialIntelligence searchable, IQueryable<ArtificialIntelligence> query)
        {
            return AddQueryArguments(searchable, query);
        }
    }
}
