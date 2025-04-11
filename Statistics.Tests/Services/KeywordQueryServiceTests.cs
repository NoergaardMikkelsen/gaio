using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Services;

[TestFixture]
public class KeywordQueryServiceTests
{
    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.Keywords.AddRange(new Keyword {Text = "Keyword 1",}, new Keyword {Text = "Keyword 2",});
        _context.SaveChanges();

        // Initialize testable service
        _service = new TestableKeywordQueryService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private StatisticsDatabaseContext _context;
    private TestableKeywordQueryService _service;

    [Test]
    public void GetBaseQuery_ShouldReturnAllKeywords()
    {
        // Act
        var result = _service.TestGetBaseQuery().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Text.Should().Be("Keyword 1");
        result[1].Text.Should().Be("Keyword 2");
    }

    [Test]
    public void AddQueryArguments_ShouldFilterByText()
    {
        // Arrange
        var searchable = new SearchableKeyword {Text = "Keyword 1",};
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Text.Should().Be("Keyword 1");
    }

    [Test]
    public async Task AddEntity_ShouldAddKeywordToDbSet()
    {
        // Arrange
        var newKeyword = new Keyword {Text = "Keyword 3",};

        // Act
        await _service.AddEntity(newKeyword);

        // Assert
        Keyword? addedKeyword = await _context.Keywords.FirstOrDefaultAsync(k => k.Text == "Keyword 3");
        addedKeyword.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateEntity_ShouldUpdateKeywordInDbSet()
    {
        // Arrange
        Keyword? updatedKeyword = await _context.Keywords.FirstAsync(k => k.Text == "Keyword 1");
        updatedKeyword.Text = "Updated Keyword 1";

        // Act
        await _service.UpdateEntity(updatedKeyword);

        // Assert
        Keyword? keyword = await _context.Keywords.FirstAsync(k => k.Text == "Updated Keyword 1");
        keyword.Should().NotBeNull();
    }

    [Test]
    public async Task DeleteEntity_ShouldRemoveKeywordFromDbSet()
    {
        // Arrange
        var searchable = new SearchableKeyword {Text = "Keyword 1",};

        // Act
        await _service.DeleteEntity(searchable);

        // Assert
        Keyword? keyword = _context.Keywords.FirstOrDefault(k => k.Text == "Keyword 1");
        keyword.Should().BeNull();
    }

    [Test]
    public async Task DeleteEntityById_ShouldRemoveKeywordById()
    {
        // Arrange
        var id = 1;

        // Act
        await _service.DeleteEntityById(id);

        // Assert
        Keyword? keyword = await _context.Keywords.FindAsync(id);
        keyword.Should().BeNull();
    }

    [Test]
    public async Task AddEntities_ShouldAddKeywordsToDbSet()
    {
        // Arrange
        var newKeywords = new List<Keyword>
        {
            new() {Text = "Keyword 3",},
            new() {Text = "Keyword 4",},
        };

        // Act
        await _service.AddEntities(newKeywords);

        // Assert
        var addedKeywords = await _context.Keywords.Where(k => k.Text.StartsWith("Keyword")).ToListAsync();
        addedKeywords.Should().HaveCount(4);
    }

    [Test]
    public async Task GetEntity_ShouldReturnMatchingKeyword()
    {
        // Arrange
        var searchable = new SearchableKeyword {Text = "Keyword 1",};

        // Act
        Keyword result = await _service.GetEntity(searchable);

        // Assert
        result.Should().NotBeNull();
        result.Text.Should().Be("Keyword 1");
    }

    [Test]
    public async Task GetEntities_ShouldReturnAllMatchingKeywords()
    {
        // Arrange
        var searchable = new SearchableKeyword {Text = "Keyword",};

        // Act
        var result = await _service.GetEntities(searchable);

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task UpdateEntities_ShouldUpdateKeywordsInDbSet()
    {
        // Arrange
        var updatedKeywords = await _context.Keywords.ToListAsync();
        updatedKeywords.ForEach(k => k.Text = "Updated " + k.Text);

        // Act
        await _service.UpdateEntities(updatedKeywords);

        // Assert
        var result = await _context.Keywords.ToListAsync();
        result.All(k => k.Text.StartsWith("Updated")).Should().BeTrue();
    }

    private class TestableKeywordQueryService : KeywordQueryService
    {
        public TestableKeywordQueryService(StatisticsDatabaseContext context) : base(context)
        {
        }

        public IQueryable<Keyword> TestGetBaseQuery()
        {
            return GetBaseQuery();
        }

        public IQueryable<Keyword> TestAddQueryArguments(SearchableKeyword searchable, IQueryable<Keyword> query)
        {
            return AddQueryArguments(searchable, query);
        }
    }
}
