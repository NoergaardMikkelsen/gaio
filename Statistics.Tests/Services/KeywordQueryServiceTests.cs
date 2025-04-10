using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Services;

[TestFixture]
public class KeywordQueryServiceTests
{
    private StatisticsDatabaseContext _context;
    private TestableKeywordQueryService _service;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.Keywords.AddRange(
            new Keyword() { Text = "Keyword 1" },
            new Keyword() { Text = "Keyword 2" }
        );
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
        var searchable = new SearchableKeyword { Text = "Keyword 1" };
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Text.Should().Be("Keyword 1");
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
