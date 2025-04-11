using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Services;

[TestFixture]
public class ArtificialIntelligenceQueryServiceTests
{
    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.ArtificialIntelligences.AddRange(
            new ArtificialIntelligence
                {Name = "AI 1", Key = "Key1", AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,},
            new ArtificialIntelligence
            {
                Name = "AI 2", Key = "Key2",
                AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,
            });
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

    private StatisticsDatabaseContext _context;
    private TestableArtificialIntelligenceQueryService _service;

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
        var searchable = new SearchableArtificialIntelligence {Name = "AI 1", Key = "Key1",};
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("AI 1");
        result[0].Key.Should().Be("Key1");
    }

    [Test]
    public async Task AddEntity_ShouldAddArtificialIntelligenceToDbSet()
    {
        // Arrange
        var newAi = new ArtificialIntelligence
            {Name = "AI 3", Key = "Key3", AiType = ArtificialIntelligenceType.OPEN_AI_WEB,};

        // Act
        await _service.AddEntity(newAi);

        // Assert
        ArtificialIntelligence? addedAi =
            await _context.ArtificialIntelligences.FirstOrDefaultAsync(ai => ai.Name == "AI 3");
        addedAi.Should().NotBeNull();
        addedAi.Key.Should().Be("Key3");
        addedAi.AiType.Should().Be(ArtificialIntelligenceType.OPEN_AI_WEB);
    }

    [Test]
    public async Task UpdateEntity_ShouldUpdateArtificialIntelligenceInDbSet()
    {
        // Arrange
        ArtificialIntelligence? updatedAi = await _context.ArtificialIntelligences.FirstAsync(ai => ai.Name == "AI 1");
        updatedAi.Key = "UpdatedKey1";

        // Act
        await _service.UpdateEntity(updatedAi);

        // Assert
        ArtificialIntelligence? ai = await _context.ArtificialIntelligences.FirstAsync(ai => ai.Name == "AI 1");
        ai.Key.Should().Be("UpdatedKey1");
    }

    [Test]
    public async Task DeleteEntity_ShouldRemoveArtificialIntelligenceFromDbSet()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {Name = "AI 1",};

        // Act
        await _service.DeleteEntity(searchable);

        // Assert
        ArtificialIntelligence? ai = _context.ArtificialIntelligences.FirstOrDefault(ai => ai.Name == "AI 1");
        ai.Should().BeNull();
    }

    [Test]
    public async Task DeleteEntityById_ShouldRemoveArtificialIntelligenceById()
    {
        // Arrange
        var id = 1;

        // Act
        await _service.DeleteEntityById(id);

        // Assert
        ArtificialIntelligence? ai = await _context.ArtificialIntelligences.FindAsync(id);
        ai.Should().BeNull();
    }

    [Test]
    public async Task AddEntities_ShouldAddArtificialIntelligencesToDbSet()
    {
        // Arrange
        var newAIs = new List<ArtificialIntelligence>
        {
            new() {Name = "AI 3", Key = "Key3", AiType = ArtificialIntelligenceType.OPEN_AI_WEB,},
            new() {Name = "AI 4", Key = "Key4", AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,},
        };

        // Act
        await _service.AddEntities(newAIs);

        // Assert
        var addedAIs = await _context.ArtificialIntelligences.Where(ai => ai.Name.StartsWith("AI")).ToListAsync();
        addedAIs.Should().HaveCount(4);
    }

    [Test]
    public async Task GetEntity_ShouldReturnMatchingArtificialIntelligence()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {Name = "AI 1",};

        // Act
        ArtificialIntelligence result = await _service.GetEntity(searchable);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("AI 1");
    }

    [Test]
    public async Task GetEntities_ShouldReturnAllMatchingArtificialIntelligences()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,};

        // Act
        var result = await _service.GetEntities(searchable);

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task UpdateEntities_ShouldUpdateArtificialIntelligencesInDbSet()
    {
        // Arrange
        var updatedAIs = await _context.ArtificialIntelligences.ToListAsync();
        updatedAIs.ForEach(ai => ai.Key = "UpdatedKey");

        // Act
        await _service.UpdateEntities(updatedAIs);

        // Assert
        var result = await _context.ArtificialIntelligences.ToListAsync();
        result.All(ai => ai.Key == "UpdatedKey").Should().BeTrue();
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

        public IQueryable<ArtificialIntelligence> TestAddQueryArguments(
            SearchableArtificialIntelligence searchable, IQueryable<ArtificialIntelligence> query)
        {
            return AddQueryArguments(searchable, query);
        }
    }
}
