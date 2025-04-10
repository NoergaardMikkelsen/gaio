using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Services;

[TestFixture]
public class ResponseQueryServiceTests
{
    private StatisticsDatabaseContext _context;
    private TestableResponseQueryService _service;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.Responses.AddRange(
            new Response() { Text = "Response 1", AiId = 1, PromptId = 1, Prompt = new Prompt() {Text = "Hello"}, Ai = new ArtificialIntelligence() {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB, Name = "Ai 1", Key = "Key 1"}},
            new Response() { Text = "Response 2", AiId = 2, PromptId = 2, Prompt = new Prompt() {Text = "Test"},
                Ai = new ArtificialIntelligence()
                    {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB, Name = "Ai 2", Key = "Key 2"}
            }
        );
        _context.SaveChanges();

        // Initialize testable service
        _service = new TestableResponseQueryService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void GetBaseQuery_ShouldReturnAllResponsesWithAiAndPrompt()
    {
        // Act
        var result = _service.TestGetBaseQuery().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Text.Should().Be("Response 1");
        result[1].Text.Should().Be("Response 2");
    }

    [Test]
    public void AddQueryArguments_ShouldFilterByAiIdAndPromptId()
    {
        // Arrange
        var searchable = new SearchableResponse { AiId = 1, PromptId = 1 };
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].AiId.Should().Be(1);
        result[0].PromptId.Should().Be(1);
    }

    [Test]
    public async Task GetEntity_ShouldReturnMatchingResponse()
    {
        // Arrange
        var searchable = new SearchableResponse { Text = "Response 1" };

        // Act
        var result = await _service.GetEntity(searchable);

        // Assert
        result.Should().NotBeNull();
        result.Text.Should().Be("Response 1");
    }

    [Test]
    public async Task GetEntities_ShouldReturnAllMatchingResponses()
    {
        // Arrange
        var searchable = new SearchableResponse { Text = "Response" };

        // Act
        var result = await _service.GetEntities(searchable);

        // Assert
        result.Should().HaveCount(2);
        result.First().Text.Should().Be("Response 1");
        result.Last().Text.Should().Be("Response 2");
    }

    private class TestableResponseQueryService : ResponseQueryService
    {
        public TestableResponseQueryService(StatisticsDatabaseContext context) : base(context)
        {
        }

        public IQueryable<Response> TestGetBaseQuery()
        {
            return GetBaseQuery();
        }

        public IQueryable<Response> TestAddQueryArguments(SearchableResponse searchable, IQueryable<Response> query)
        {
            return AddQueryArguments(searchable, query);
        }
    }
}
